using AppEmocions.Api.Data.Jocs;
using Microsoft.AspNetCore.Mvc;

namespace AppEmocions.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class JocsController : ControllerBase
{
    private readonly JocsRepository _repo;

    public JocsController(JocsRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos() => Ok(await _repo.ObtenerTodosAsync());

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] GuardarJuegoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre))
            return BadRequest("El nombre del juego es obligatorio.");

        var id = await _repo.CrearAsync(request);
        return Ok(new { juego_id = id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] GuardarJuegoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre))
            return BadRequest("El nombre del juego es obligatorio.");

        return await _repo.ActualizarAsync(id, request) ? NoContent() : NotFound();
    }

    [HttpPatch("{id:int}/activo")]
    public async Task<IActionResult> CambiarEstado(int id, [FromBody] bool activo)
        => await _repo.CambiarEstadoAsync(id, activo) ? NoContent() : NotFound();
}