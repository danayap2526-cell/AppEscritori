
using AppEmocions.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace AppEmocions.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SesionesController : ControllerBaseDatos
{
    public SesionesController(Db db) : base(db) { }

    // GET: api/sesiones
    [HttpGet]
    public IActionResult ObtenerTodas()
    {
        const string sql = @"
            SELECT
                s.sesion_id,
                s.participante_id,
                p.codigo AS codigo_participante,
                p.grupo,
                s.juego_id,
                j.nombre AS nombre_juego,
                s.schema_version,
                s.iniciada_en,
                s.fecha_local,
                s.duracion_s,
                s.puntuacion,
                s.nivel_alcanzado,
                s.intentos,
                s.errores,
                s.ayudas,
                s.resp_media_ms,
                s.completada,
                s.abandonada,
                s.simulada,
                s.aciertos,
                s.dificultad

            FROM dbo.sesion s
            INNER JOIN dbo.participante p
                ON p.participante_id = s.participante_id
            INNER JOIN dbo.juego j
                ON j.juego_id = s.juego_id

            ORDER BY s.iniciada_en DESC;";

        return Ok(Convertir(_db.Consultar(sql)));
    }

    // GET: api/sesiones/{id}
    [HttpGet("{id}")]
    public IActionResult ObtenerPorId(string id)
    {
        const string sql = @"
            SELECT
                s.sesion_id,
                s.participante_id,
                p.codigo AS codigo_participante,
                p.grupo,
                s.juego_id,
                j.nombre AS nombre_juego,
                s.schema_version,
                s.iniciada_en,
                s.iniciada_utc,
                s.fecha_local,
                s.duracion_s,
                s.puntuacion,
                s.nivel_alcanzado,
                s.intentos,
                s.errores,
                s.ayudas,
                s.resp_media_ms,
                s.completada,
                s.abandonada,
                s.simulada,
                s.aciertos,
                s.dificultad

            FROM dbo.sesion s
            INNER JOIN dbo.participante p
                ON p.participante_id = s.participante_id
            INNER JOIN dbo.juego j
                ON j.juego_id = s.juego_id

            WHERE s.sesion_id = @id;";

        var tabla = _db.Consultar(sql,
            new SqlParameter("@id", id));

        var resultado = Convertir(tabla);

        if (resultado.Count == 0)
            return NotFound("Sesión no encontrada.");

        return Ok(resultado[0]);
    }
}
