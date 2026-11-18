
using AppEmocions.Api.Data;
using Microsoft.AspNetCore.Mvc;

namespace AppEmocions.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmocionesController : ControllerBase
{
    private readonly Db _db;

    public EmocionesController(Db db)
    {
        _db = db;
    }

    // GET: api/emociones/estado
    [HttpGet("estado")]
    public IActionResult Estado()
    {
        if (_db.Probar(out string mensaje))
            return Ok(mensaje);

        return StatusCode(503, mensaje);
    }
}
