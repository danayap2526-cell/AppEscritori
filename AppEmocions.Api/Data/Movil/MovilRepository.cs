using System.Data;
using Dapper;

namespace AppEmocions.Api.Data.Movil;

public sealed class MovilRepository
{
    private readonly DbConnectionFactory _db;

    public MovilRepository(DbConnectionFactory db) => _db = db;

    public async Task<string> GuardarSesionAsync(SesionMovilDto d)
    {
        var sesionId = Guid.NewGuid().ToString();
        var ahoraUtc = DateTime.UtcNow;
        var inicioUtc = ahoraUtc.AddSeconds(-d.DuracionS);
        var inicioLocal = DateTime.Now.AddSeconds(-d.DuracionS);

        string? dificultad = string.IsNullOrWhiteSpace(d.Dificultad)
            ? null
            : d.Dificultad.Trim().Length > 10 ? d.Dificultad.Trim()[..10] : d.Dificultad.Trim();

        const string sql = @"
            SET XACT_ABORT ON;
            BEGIN TRAN;

            DECLARE @pid INT = (
                SELECT TOP 1 participante_id
                FROM dbo.participante
                WHERE nombre = @Usuario OR codigo = @Usuario
            );

            IF @pid IS NULL
            BEGIN
                DECLARE @Tmp TABLE (Creado INT, ParticipanteId INT, Codigo NVARCHAR(20));
                INSERT INTO @Tmp EXEC dbo.sp_RegistrarParticipanteMovil
                    @Nombre = @Usuario, @ContrasenaHash = '', @Grupo = @Grupo;
                SELECT @pid = ParticipanteId FROM @Tmp;
            END

            INSERT INTO dbo.sesion
                (sesion_id, participante_id, juego_id, schema_version, iniciada_en, iniciada_utc, fecha_local,
                 duracion_s, puntuacion, nivel_alcanzado, intentos, errores, ayudas, resp_media_ms,
                 completada, abandonada, simulada, aciertos, dificultad)
            VALUES
                (@Sesion, @pid, 1, 1, @InicioLocal, @InicioUtc, CAST(@InicioLocal AS date),
                 @Dur, @Punt, @Nivel, @Intentos, @Errores, @Ayudas, @RespMediaMs,
                 @Completada, 0, @Simulada, @Aciertos, @Dificultad);

            INSERT INTO dbo.nivel_sesion
                (sesion_id, nivel, completado, duracion_ms, intentos, errores, ayudas, resp_media_ms,
                 err_alegria, err_tristeza, err_ira, err_miedo,
                 n_abrazar, n_compartir, n_avisar_adulto, ciclos_completados, aciertos)
            VALUES
                (@Sesion, @Nivel, @Completada, @Dur * 1000, @Intentos, @Errores, @Ayudas, @RespMediaMs,
                 0, 0, 0, 0, 0, 0, 0, 0, @Aciertos);

            COMMIT;";

        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync(sql, new
        {
            Usuario = d.Codigo.Trim(),
            Grupo = string.IsNullOrWhiteSpace(d.Grupo) ? "P4-A" : d.Grupo.Trim(),
            Sesion = sesionId,
            InicioLocal = inicioLocal,
            InicioUtc = inicioUtc,
            Dur = d.DuracionS,
            Punt = d.Puntuacion,
            Nivel = (byte)d.Nivel,
            d.Intentos,
            d.Errores,
            d.Ayudas,
            d.RespMediaMs,
            d.Completada,
            d.Simulada,
            d.Aciertos,
            Dificultad = dificultad
        });

        return sesionId;
    }

    public async Task<IEnumerable<SesionMovilHistorialDto>> ObtenerSesionesDeAsync(string usuario)
    {
        const string sql = @"
            SELECT s.sesion_id, s.iniciada_en, s.duracion_s, s.puntuacion, s.nivel_alcanzado, s.errores, s.aciertos
            FROM dbo.sesion s
            INNER JOIN dbo.participante p ON p.participante_id = s.participante_id
            WHERE p.nombre = @Usuario OR p.codigo = @Usuario
            ORDER BY s.iniciada_en DESC;";

        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<SesionMovilHistorialDto>(sql, new { Usuario = usuario.Trim() });
    }

    public async Task<bool> ValidarLoginAsync(string username, string passwordHash)
    {
        const string sql = @"
            SELECT COUNT(1)
            FROM dbo.participante
            WHERE nombre = @Nombre
              AND contrasena_hash = @PasswordHash
              AND activo = 1;";

        using var conn = _db.CreateConnection();
        return await conn.ExecuteScalarAsync<int>(sql, new
        {
            Nombre = username.Trim(),
            PasswordHash = passwordHash
        }) > 0;
    }

    public async Task<(bool Creado, string? Codigo)> RegistrarParticipanteAsync(string username, string passwordHash)
    {
        using var conn = _db.CreateConnection();
        var res = await conn.QueryFirstAsync<(int Creado, int? ParticipanteId, string? Codigo)>(
            "dbo.sp_RegistrarParticipanteMovil",
            new { Nombre = username.Trim(), ContrasenaHash = passwordHash },
            commandType: CommandType.StoredProcedure);

        return (res.Creado == 1, res.Codigo);
    }
}