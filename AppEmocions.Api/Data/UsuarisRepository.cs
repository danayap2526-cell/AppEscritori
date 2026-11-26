using AppEmocions.Api.Dtos.Usuaris;

using System.Security.Cryptography;
using System.Text;
using Dapper;

namespace AppEmocions.Api.Data;

public sealed class UsuarisRepository
{
    private readonly DbConnectionFactory _db;

    public UsuarisRepository(DbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<DocenteDto>> ObtenerDocentesAsync()
    {
        using var conn = _db.CreateConnection();

        const string sqlDocentes = @"
            SELECT docente_id, usuario, nombre_visible, email, rol, activo, ultimo_acceso
            FROM dbo.docente
            ORDER BY nombre_visible;";

        var docentes = (await conn.QueryAsync<DocenteDto>(sqlDocentes)).ToList();

        const string sqlGrupos = "SELECT docente_id, grupo FROM dbo.docente_grupo;";
        var grupos = await conn.QueryAsync<(int DocenteId, string Grupo)>(sqlGrupos);

        var mapaGrupos = grupos.GroupBy(g => g.DocenteId)
                               .ToDictionary(g => g.Key, g => g.Select(x => x.Grupo).ToList());

        foreach (var d in docentes)
        {
            if (mapaGrupos.TryGetValue(d.DocenteId, out var lista))
                d.Grupos = lista;
        }

        return docentes;
    }

    public async Task<int> CrearDocenteAsync(CrearDocenteRequest req)
    {
        using var conn = _db.CreateConnection();
        conn.Open();
        using var tx = conn.BeginTransaction();

        try
        {
            const string sqlInsert = @"
                INSERT INTO dbo.docente (usuario, nombre_visible, contrasena_hash, rol, activo, email)
                VALUES (@Usuario, @NombreVisible, @Hash, @Rol, 1, @Email);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            int docenteId = await conn.QuerySingleAsync<int>(sqlInsert, new
            {
                Usuario = req.Usuario.Trim(),
                NombreVisible = req.NombreVisible.Trim(),
                Hash = HashPassword(req.Contrasena),
                Rol = req.Rol.Trim(),
                Email = string.IsNullOrWhiteSpace(req.Email) ? null : req.Email.Trim()
            }, tx);

            foreach (var grupo in req.Grupos.Distinct())
            {
                await conn.ExecuteAsync(
                    "INSERT INTO dbo.docente_grupo (docente_id, grupo) VALUES (@DocenteId, @Grupo);",
                    new { DocenteId = docenteId, Grupo = grupo.Trim() },
                    tx);
            }

            tx.Commit();
            return docenteId;
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    public async Task<bool> CambiarEstadoDocenteAsync(int docenteId, bool activo)
    {
        const string sql = "UPDATE dbo.docente SET activo = @Activo WHERE docente_id = @Id;";
        using var conn = _db.CreateConnection();
        return await conn.ExecuteAsync(sql, new { Id = docenteId, Activo = activo }) > 0;
    }

    public async Task<bool> CambiarContrasenaAsync(int docenteId, CambiarContrasenaRequest req)
    {
        using var conn = _db.CreateConnection();

        const string sqlCheck = "SELECT contrasena_hash FROM dbo.docente WHERE docente_id = @Id;";
        var hashActual = await conn.QueryFirstOrDefaultAsync<string>(sqlCheck, new { Id = docenteId });

        if (hashActual is null || hashActual != HashPassword(req.ContrasenaActual))
            return false;

        const string sqlUpdate = "UPDATE dbo.docente SET contrasena_hash = @NuevoHash WHERE docente_id = @Id;";
        await conn.ExecuteAsync(sqlUpdate, new { Id = docenteId, NuevoHash = HashPassword(req.ContrasenaNueva) });
        return true;
    }

    public async Task<(DocenteDto? Docente, string? Error)> ValidarLoginAsync(string usuario, string contrasena)
    {
        using var conn = _db.CreateConnection();

        const string sql = @"
            SELECT docente_id, usuario, nombre_visible, contrasena_hash, email, rol, activo, ultimo_acceso
            FROM dbo.docente
            WHERE usuario = @Usuario;";

        var row = await conn.QueryFirstOrDefaultAsync<dynamic>(sql, new { Usuario = usuario.Trim() });
        if (row is null)
            return (null, "Usuari o contrasenya incorrectes.");

        string hashGuardado = (string?)row.contrasena_hash ?? string.Empty;
        string hashIntroducido = HashPassword(contrasena);

        if (!string.Equals(hashGuardado, hashIntroducido, StringComparison.OrdinalIgnoreCase))
            return (null, "Usuari o contrasenya incorrectes.");

        bool activo = Convert.ToBoolean(row.activo);
        if (!activo)
            return (null, "Aquest compte d'usuari estÃ  desactivat.");

        int docenteId = (int)row.docente_id;
        await conn.ExecuteAsync(
            "UPDATE dbo.docente SET ultimo_acceso = GETDATE() WHERE docente_id = @Id;",
            new { Id = docenteId });

        var grupos = (await conn.QueryAsync<string>(
            "SELECT grupo FROM dbo.docente_grupo WHERE docente_id = @Id;",
            new { Id = docenteId })).ToList();

        var docente = new DocenteDto
        {
            DocenteId = docenteId,
            Usuario = (string)row.usuario,
            NombreVisible = (string)row.nombre_visible,
            Email = (string?)row.email,
            Rol = (string)row.rol,
            Activo = true,
            UltimoAcceso = DateTime.Now,
            Grupos = grupos
        };

        return (docente, null);
    }

    private static string HashPassword(string plain)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(plain));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}