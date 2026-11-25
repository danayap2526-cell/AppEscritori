using AppEmocions.Api.Dtos.Dashboard;

using Dapper;

namespace AppEmocions.Api.Data;

public sealed class DashboardRepository
{
    private readonly DbConnectionFactory _db;

    public DashboardRepository(DbConnectionFactory db) => _db = db;

    private const string CteSesiones = @"
        WITH ses AS (
            SELECT
                s.sesion_id,
                s.participante_id,
                p.codigo,
                p.grupo,
                s.iniciada_en,
                s.puntuacion,
                s.resp_media_ms,
                s.completada,
                s.abandonada,
                COALESCE(s.aciertos, 0) AS aciertos,
                COALESCE(s.errores, 0)  AS errores
            FROM dbo.sesion s
            INNER JOIN dbo.participante p ON p.participante_id = s.participante_id
            WHERE s.simulada = 0
              AND s.iniciada_en >= @Desde
              AND s.iniciada_en <  @HastaExclusivo
              AND (@Grupo IS NULL OR p.grupo = @Grupo)
        )";

    public async Task<ResumenDashboardDto> ObtenerResumenAsync(FiltroDashboard f)
    {
        const string sql = CteSesiones + @"
            SELECT
                (SELECT COUNT(DISTINCT participante_id) FROM ses) AS participantes_activos,
                (SELECT COUNT(*) FROM ses) AS sesiones,
                (SELECT COUNT(*) FROM ses WHERE completada = 1) AS sesiones_completadas,
                (SELECT COUNT(*) FROM ses WHERE abandonada = 1) AS sesiones_abandonadas,
                COALESCE((SELECT AVG(CAST(puntuacion AS FLOAT)) FROM ses), 0) AS puntuacion_media,
                COALESCE((SELECT AVG(CAST(resp_media_ms AS FLOAT)) FROM ses WHERE resp_media_ms IS NOT NULL), 0) AS resp_media_ms,
                COALESCE((SELECT SUM(ns.aciertos) FROM dbo.nivel_sesion ns INNER JOIN ses ON ses.sesion_id = ns.sesion_id WHERE ns.nivel IN (1, 2)), 0) AS aciertos,
                COALESCE((SELECT SUM(ns.errores)  FROM dbo.nivel_sesion ns INNER JOIN ses ON ses.sesion_id = ns.sesion_id WHERE ns.nivel IN (1, 2)), 0) AS errores;";

        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<ResumenDashboardDto>(sql, f);
    }

    public async Task<IEnumerable<SemanaEvolucionDto>> ObtenerSemanasAsync(FiltroDashboard f)
    {
        const string sql = CteSesiones + @"
            SELECT
                semana,
                grupo,
                COALESCE(SUM(aciertos), 0) AS aciertos,
                COALESCE(SUM(errores), 0)  AS errores,
                CASE WHEN SUM(aciertos + errores) > 0
                     THEN CAST(SUM(aciertos) * 100.0 / SUM(aciertos + errores) AS FLOAT)
                     ELSE 0 END AS tasa_acierto
            FROM (
                SELECT
                    DATEDIFF(DAY, @Desde, ses.iniciada_en) / 7 + 1 AS semana,
                    ses.grupo,
                    ns.aciertos,
                    ns.errores
                FROM dbo.nivel_sesion ns
                INNER JOIN ses ON ses.sesion_id = ns.sesion_id
                WHERE ns.nivel IN (1, 2)
            ) t
            GROUP BY semana, grupo
            ORDER BY semana, grupo;";

        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<SemanaEvolucionDto>(sql, f);
    }

    public async Task<IEnumerable<NivelEstadisticaDto>> ObtenerNivelesAsync(FiltroDashboard f)
    {
        const string sql = CteSesiones + @"
            SELECT
                ns.nivel,
                COALESCE(SUM(ns.aciertos), 0) AS aciertos,
                COALESCE(SUM(ns.errores), 0)  AS errores,
                COUNT(*) AS registros,
                SUM(CASE WHEN ns.completado = 1 THEN 1 ELSE 0 END) AS completados
            FROM dbo.nivel_sesion ns
            INNER JOIN ses ON ses.sesion_id = ns.sesion_id
            GROUP BY ns.nivel
            ORDER BY ns.nivel;";

        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<NivelEstadisticaDto>(sql, f);
    }

    public async Task<IEnumerable<MetricaAtencionRawDto>> ObtenerMetricasAtencionAsync(FiltroDashboard f)
    {
        const string sql = CteSesiones + @"
            SELECT
                codigo,
                SUM(CASE WHEN abandonada = 1 THEN 1 ELSE 0 END) AS interrumpidas,
                AVG(CAST(resp_media_ms AS FLOAT)) AS resp_ms,
                SUM(CASE WHEN iniciada_en >= DATEADD(DAY, -7, @HastaExclusivo) THEN aciertos ELSE 0 END) AS ac_rec,
                SUM(CASE WHEN iniciada_en >= DATEADD(DAY, -7, @HastaExclusivo) THEN aciertos + errores ELSE 0 END) AS tot_rec,
                SUM(CASE WHEN iniciada_en <  DATEADD(DAY, -7, @HastaExclusivo) THEN aciertos ELSE 0 END) AS ac_ant,
                SUM(CASE WHEN iniciada_en <  DATEADD(DAY, -7, @HastaExclusivo) THEN aciertos + errores ELSE 0 END) AS tot_ant
            FROM ses
            GROUP BY codigo;";

        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<MetricaAtencionRawDto>(sql, f);
    }

    public async Task<IEnumerable<string>> ObtenerGruposAsync()
    {
        const string sql = @"
            SELECT DISTINCT LTRIM(RTRIM(grupo)) AS grupo
            FROM dbo.docente_grupo
            WHERE grupo IS NOT NULL AND LTRIM(RTRIM(grupo)) <> ''
            ORDER BY grupo;";

        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<string>(sql);
    }

    public async Task<IEnumerable<ComparativaGrupoDto>> ObtenerComparativaGruposAsync(FiltroDashboard f)
    {
        const string sql = @"
            SELECT
                p.grupo,
                COUNT(DISTINCT p.participante_id) AS total_infants,
                COUNT(s.sesion_id) AS sesiones,
                COALESCE(SUM(s.aciertos) * 100.0 / NULLIF(SUM(s.aciertos + s.errores), 0), 0) AS tasa_acierto,
                COALESCE(AVG(CAST(s.resp_media_ms AS FLOAT)), 0) AS resp_media_ms,
                COALESCE(SUM(CASE WHEN s.abandonada = 1 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(s.sesion_id), 0), 0) AS tasa_abandono,
                COALESCE(SUM(CASE WHEN s.completada = 1 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(s.sesion_id), 0), 0) AS tasa_finalizacion
            FROM dbo.participante p
            LEFT JOIN dbo.sesion s
                ON s.participante_id = p.participante_id
               AND s.simulada = 0
               AND s.iniciada_en >= @Desde
               AND s.iniciada_en <  @HastaExclusivo
            WHERE p.activo = 1
              AND (@Grupo IS NULL OR p.grupo = @Grupo)
            GROUP BY p.grupo
            ORDER BY p.grupo;";

        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<ComparativaGrupoDto>(sql, f);
    }

    public async Task<IEnumerable<ComparativaNivelGrupoDto>> ObtenerComparativaNivelesAsync(FiltroDashboard f)
    {
        const string sql = CteSesiones + @"
            SELECT
                ses.grupo,
                ns.nivel,
                COALESCE(SUM(ns.aciertos) * 100.0 / NULLIF(SUM(ns.aciertos + ns.errores), 0), 0) AS tasa_acierto
            FROM dbo.nivel_sesion ns
            INNER JOIN ses ON ses.sesion_id = ns.sesion_id
            GROUP BY ses.grupo, ns.nivel
            ORDER BY ns.nivel, ses.grupo;";

        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<ComparativaNivelGrupoDto>(sql, f);
    }

    public async Task<IEnumerable<AbandonoNivelGrupoDto>> ObtenerAbandonoPorNivelAsync(FiltroDashboard f)
    {
        const string sql = CteSesiones + @"
            SELECT
                ns.nivel,
                ses.grupo,
                COUNT(*) AS iniciadas,
                SUM(CASE WHEN ns.completado = 1 THEN 1 ELSE 0 END) AS completadas,
                SUM(CASE WHEN ns.completado = 0 AND ns.motiu_abandonament LIKE '%Descans%' THEN 1 ELSE 0 END) AS descansar,
                SUM(CASE WHEN ns.completado = 0 AND ns.motiu_abandonament LIKE '%Sortida%' THEN 1 ELSE 0 END) AS sortida,
                SUM(CASE WHEN ns.completado = 0 AND (ns.motiu_abandonament LIKE '%Segon pla%' OR ns.motiu_abandonament IS NULL) THEN 1 ELSE 0 END) AS segon_pla
            FROM dbo.nivel_sesion ns
            INNER JOIN ses ON ses.sesion_id = ns.sesion_id
            GROUP BY ns.nivel, ses.grupo
            ORDER BY ns.nivel, ses.grupo;";

        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<AbandonoNivelGrupoDto>(sql, f);
    }
}