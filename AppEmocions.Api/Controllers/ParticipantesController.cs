
using AppEmocions.Api.Data;
using Microsoft.AspNetCore.Mvc;

namespace AppEmocions.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParticipantesController : ControllerBaseDatos
{
    public ParticipantesController(Db db) : base(db)
    {
    }

    // GET: api/participantes
    [HttpGet]
    public IActionResult ObtenerTodos()
    {
        const string sql = @"
            SELECT
                participante_id,
                codigo,
                grupo,
                activo
            FROM dbo.participante
            ORDER BY participante_id;";

        var tabla = _db.Consultar(sql);
        return Ok(Convertir(tabla));
    }

    // GET: api/participantes/5
    [HttpGet("{id:int}")]
    public IActionResult ObtenerPorId(int id)
    {
        const string sql = @"
            SELECT
                participante_id,
                codigo,
                grupo,
                activo
            FROM dbo.participante
            WHERE participante_id = @id;";

        var tabla = _db.Consultar(sql,
            new Microsoft.Data.SqlClient.SqlParameter("@id", id));

        var resultado = Convertir(tabla);

        if (resultado.Count == 0)
            return NotFound("No se ha encontrado el participante.");

        return Ok(resultado[0]);
    }
}
