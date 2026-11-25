using Dapper;

namespace AppEmocions.Api.Data.Dades;

public sealed class DadesRepository
{
    private readonly DbConnectionFactory _db;

    public DadesRepository(DbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<SesionImportadaDto>> ObtenerSesionesAsync(bool incluirSimuladas)
    {
        const string sql = @"
            SELECT TOP 100
                s.sesion_id,
                p.codigo AS codigo_infant,
                s.iniciada_en AS data,
                j.nombre AS joc,
                s.simulada,
                'OK' AS estat
            FROM dbo.sesion s
            INNER JOIN dbo.participante p ON p.participante_id = s.participante_id
            INNER JOIN dbo.juego j ON j.juego_id = s.juego_id
            WHERE (@IncluirSimuladas = 1 OR s.simulada = 0)
            ORDER BY s.iniciada_en DESC;";

        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<SesionImportadaDto>(sql, new { IncluirSimuladas = incluirSimuladas ? 1 : 0 });
    }

    public async Task<ResumenCalidadDatosDto> ObtenerControlCoherenciaAsync()
    {
        using var conn = _db.CreateConnection();

        int total = await conn.QuerySingleAsync<int>("SELECT COUNT(*) FROM dbo.sesion WHERE simulada = 0;");

        const string sqlIncidencias = @"
            SELECT
                'Sessió sense hora final' AS tipo,
                sesion_id AS referencia,
                'Duració nul·la' AS detalle
            FROM dbo.sesion
            WHERE simulada = 0 AND duracion_s IS NULL
            UNION ALL
            SELECT
                'Durada superior a 30 min' AS tipo,
                sesion_id AS referencia,
                CAST(duracion_s / 60 AS VARCHAR(10)) + ' min' AS detalle
            FROM dbo.sesion
            WHERE simulada = 0 AND duracion_s > 1800;";

        var incidencias = (await conn.QueryAsync<IncidenciaCoherenciaDto>(sqlIncidencias)).ToList();
        int sinIncidencias = Math.Max(0, total - incidencias.Count);
        double porcentaje = total > 0 ? Math.Round(sinIncidencias * 100.0 / total, 1) : 100.0;

        return new ResumenCalidadDatosDto(total, sinIncidencias, porcentaje, incidencias);
    }
}