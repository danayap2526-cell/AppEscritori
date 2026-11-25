using System.Data;
using Dapper;

namespace AppEmocions.Api.Data.Infants;

public sealed class InfantsRepository
{
    private readonly DbConnectionFactory _db;

    public InfantsRepository(DbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<InfantListadoDto>> ObtenerListadoAsync(
        DateTime desde, DateTime hastaExclusivo, string? grupo, string? busqueda, bool soloActivos)
    {
        const string sql = @"
            SELECT
                p.participante_id,
                p.codigo,
                p.nombre,
                p.grupo,
                p.docente_id,
                d.nombre_visible AS nombre_docente,
                p.activo,
                COUNT(s.sesion_id) AS sessions,
                COUNT(DISTINCT s.fecha_local) AS dies_actius,
                MAX(s.iniciada_en) AS ultima_sessio,
                COALESCE(SUM(s.aciertos) * 100.0 / NULLIF(SUM(s.aciertos + s.errores), 0), 0) AS taxa_encert,
                COALESCE(AVG(CAST(s.resp_media_ms AS FLOAT)), 0) AS temps_resposta_ms,
                COALESCE(SUM(CASE WHEN s.abandonada = 1 THEN 1 ELSE 0 END), 0) AS abandonaments
            FROM dbo.participante p
            LEFT JOIN dbo.docente d ON d.docente_id = p.docente_id
            LEFT JOIN dbo.sesion s
                ON s.participante_id = p.participante_id
               AND s.simulada = 0
               AND s.iniciada_en >= @Desde
               AND s.iniciada_en <  @HastaExclusivo
            WHERE (@SoloActivos = 0 OR p.activo = 1)
              AND (@Grupo IS NULL OR p.grupo = @Grupo)
              AND (@Busqueda IS NULL OR p.codigo LIKE '%' + @Busqueda + '%' OR p.nombre LIKE '%' + @Busqueda + '%')
            GROUP BY p.participante_id, p.codigo, p.nombre, p.grupo, p.docente_id, d.nombre_visible, p.activo
            ORDER BY p.codigo;";

        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<InfantListadoDto>(sql, new
        {
            Desde = desde,
            HastaExclusivo = hastaExclusivo,
            Grupo = string.IsNullOrWhiteSpace(grupo) ? null : grupo.Trim(),
            Busqueda = string.IsNullOrWhiteSpace(busqueda) ? null : busqueda.Trim(),
            SoloActivos = soloActivos ? 1 : 0
        });
    }

    public async Task<InfantDetalleCompletoDto?> ObtenerDetalleAsync(
        string codigo, DateTime desde, DateTime hastaExclusivo)
    {
        using var conn = _db.CreateConnection();

        const string sqlCabecera = @"
            SELECT
                p.participante_id,
                p.codigo,
                p.nombre,
                p.grupo,
                p.docente_id,
                d.nombre_visible AS nombre_docente,
                p.activo,
                MIN(s.iniciada_en) AS primera_sessio,
                MAX(s.iniciada_en) AS ultima_sessio,
                COUNT(s.sesion_id) AS sessions,
                COALESCE(SUM(s.aciertos) * 100.0 / NULLIF(SUM(s.aciertos + s.errores), 0), 0) AS taxa_encert,
                COALESCE(AVG(CAST(s.resp_media_ms AS FLOAT)), 0) AS temps_resposta_ms
            FROM dbo.participante p
            LEFT JOIN dbo.docente d ON d.docente_id = p.docente_id
            LEFT JOIN dbo.sesion s
                ON s.participante_id = p.participante_id
               AND s.simulada = 0
               AND s.iniciada_en >= @Desde
               AND s.iniciada_en <  @HastaExclusivo
            WHERE p.codigo = @Codigo
            GROUP BY p.participante_id, p.codigo, p.nombre, p.grupo, p.docente_id, d.nombre_visible, p.activo;";

        var cabecera = await conn.QueryFirstOrDefaultAsync<InfantDetalleCabeceraDto>(
            sqlCabecera, new { Codigo = codigo.Trim(), Desde = desde, HastaExclusivo = hastaExclusivo });

        if (cabecera is null)
            return null;

        const string sqlNiveles = @"
            SELECT
                ns.nivel,
                COUNT(*) AS registros,
                SUM(CASE WHEN ns.completado = 1 THEN 1 ELSE 0 END) AS completados,
                COALESCE(SUM(ns.aciertos), 0) AS aciertos,
                COALESCE(SUM(ns.errores), 0) AS errores,
                COALESCE(AVG(CAST(ns.resp_media_ms AS FLOAT)), 0) AS resp_media_ms
            FROM dbo.nivel_sesion ns
            INNER JOIN dbo.sesion s ON s.sesion_id = ns.sesion_id
            WHERE s.participante_id = @ParticipanteId
              AND s.simulada = 0
              AND s.iniciada_en >= @Desde
              AND s.iniciada_en <  @HastaExclusivo
            GROUP BY ns.nivel
            ORDER BY ns.nivel;";

        var niveles = await conn.QueryAsync<InfantNivelDto>(
            sqlNiveles, new { cabecera.ParticipanteId, Desde = desde, HastaExclusivo = hastaExclusivo });

        const string sqlSesiones = @"
            SELECT
                s.sesion_id,
                j.nombre AS nombre_juego,
                s.iniciada_en,
                s.duracion_s,
                s.puntuacion,
                s.nivel_alcanzado,
                s.completada,
                s.abandonada,
                s.aciertos,
                s.errores
            FROM dbo.sesion s
            INNER JOIN dbo.juego j ON j.juego_id = s.juego_id
            WHERE s.participante_id = @ParticipanteId
              AND s.simulada = 0
              AND s.iniciada_en >= @Desde
              AND s.iniciada_en <  @HastaExclusivo
            ORDER BY s.iniciada_en DESC;";

        var sesiones = await conn.QueryAsync<InfantSesionDto>(
            sqlSesiones, new { cabecera.ParticipanteId, Desde = desde, HastaExclusivo = hastaExclusivo });

        return new InfantDetalleCompletoDto(cabecera, niveles, sesiones);
    }

    public async Task<(bool Creado, int? ParticipanteId, string? Codigo)> CrearConSpAsync(CrearInfantRequest req)
    {
        using var conn = _db.CreateConnection();
        var res = await conn.QueryFirstAsync<(int Creado, int? ParticipanteId, string? Codigo)>(
            "dbo.sp_RegistrarParticipanteMovil",
            new
            {
                Nombre = req.Nombre.Trim(),
                ContrasenaHash = req.ContrasenaHash ?? string.Empty,
                Grupo = req.Grupo.Trim(),
                DocenteId = req.DocenteId
            },
            commandType: CommandType.StoredProcedure);

        return (res.Creado == 1, res.ParticipanteId, res.Codigo);
    }

    public async Task<bool> AsignarDocenteAsync(int participanteId, AsignarDocenteRequest req)
    {
        const string sql = @"
            UPDATE dbo.participante
            SET docente_id = @DocenteId,
                grupo = COALESCE(@Grupo, grupo)
            WHERE participante_id = @ParticipanteId;";

        using var conn = _db.CreateConnection();
        return await conn.ExecuteAsync(sql, new
        {
            ParticipanteId = participanteId,
            req.DocenteId,
            Grupo = string.IsNullOrWhiteSpace(req.Grupo) ? null : req.Grupo.Trim()
        }) > 0;
    }
}