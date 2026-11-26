using AppEmocions.Api.Dtos.Jocs;

using Dapper;

namespace AppEmocions.Api.Data;

public sealed class JocsRepository(DbConnectionFactory db)
{
    public async Task<IEnumerable<JuegoDto>> ObtenerTodosAsync()
    {
        using var conn = db.CreateConnection();
        return await conn.QueryAsync<JuegoDto>("SELECT juego_id AS JuegoId, nombre AS Nombre, descripcion AS Descripcion, activo AS Activo FROM dbo.Juego ORDER BY juego_id;");
    }

    public async Task<int> CrearAsync(GuardarJuegoRequest req)
    {
        using var conn = db.CreateConnection();
        return await conn.ExecuteScalarAsync<int>("INSERT SNTO dbo.Juegos (nombre, descripcion, activo) OUTPUT INSERTED.juego_id VALUES (@Nombre, @Descripcion, @Activo);", new { Nombre = req.Nombre.Trim(), Descripcion = req.Descripcion?.Trim(), req.Activo });
    }

    public async Task<bool> ActualizarAsync(int id, GuardarJuegoRequest req)
    {
        using var conn = db.CreateConnection();
        var filas = await conn.ExecuteAsync("UPDATE dbo.Juegos SET nombre = @Nombre, descripcion = @Descripcion, activo = @Activo WHERE juego_id = @Id;", new { Id = id, Nombre = req.Nombre.Trim(), Descripcion = req.Descripcion?.Trim(), req.Activo });
        return filas > 0;
    }

    public async Task<bool> CambiarEstadoAsync(int id, bool activo)
    {
        using var conn = db.CreateConnection();
        var filas = await conn.ExecuteAsync("UPDATE dbo.Juegos SET activo = @Activo WHERE juego_id = @Id;", new { Id = id, Activo = activo });
        return filas > 0;
    }
}
