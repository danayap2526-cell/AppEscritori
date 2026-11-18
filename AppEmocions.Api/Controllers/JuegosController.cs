
using AppEmocions.Api.Data;
using Microsoft.AspNetCore.Mvc;

namespace AppEmocions.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JuegosController : ControllerBaseDatos
{
    public JuegosController(Db db) : base(db) { }

    // GET: api/juegos
    [HttpGet]
    public IActionResult ObtenerTodos()
    {
        const string sql = @"
            SELECT
                juego_id,
                nombre,
                descripcion,
                activo
            FROM dbo.juego
            ORDER BY nombre;";

        return Ok(Convertir(_db.Consultar(sql)));
    }
}
