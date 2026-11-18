using AppEmocions.Api.Data;
using Microsoft.AspNetCore.Mvc;

namespace AppEmocions.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmocionesController : ControllerBase
{
    private readonly Db _db;
    public EmocionesController(Db db) => _db = db;

    [HttpGet("estado")]
    public IActionResult Estado() =>
    _db.Probar(out var msg) ? Ok(msg) : StatusCode(503, msg);
}