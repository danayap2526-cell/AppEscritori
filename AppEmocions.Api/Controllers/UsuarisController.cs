using AppEmocions.Api.Data;
using AppEmocions.Api.Dtos.Usuaris;
using Microsoft.AspNetCore.Mvc;

namespace AppEmocions.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class UsuarisController : ControllerBase
{
    private readonly UsuarisRepository _repo;

    public UsuarisController(UsuarisRepository repo) => _repo = repo;

    [HttpGet("docentes")]
    public async Task<IActionResult> ObtenerDocentes() => Ok(await _repo.ObtenerDocentesAsync());

    [HttpPost("docentes")]
    public async Task<IActionResult> CrearDocente([FromBody] CrearDocenteRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Usuario) || string.IsNullOrWhiteSpace(request.Contrasena))
            return BadRequest("Usuario y contraseña obligatorios.");

        var id = await _repo.CrearDocenteAsync(request);
        return Ok(new { docente_id = id });
    }

    [HttpPatch("docentes/{id:int}/activo")]
    public async Task<IActionResult> CambiarEstado(int id, [FromBody] bool activo)
        => await _repo.CambiarEstadoDocenteAsync(id, activo) ? NoContent() : NotFound();

    [HttpPost("docentes/{id:int}/contrasena")]
    public async Task<IActionResult> CambiarContrasena(int id, [FromBody] CambiarContrasenaRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ContrasenaNueva) || request.ContrasenaNueva.Length < 10)
            return BadRequest("La nueva contraseña debe tener al menos 10 caracteres.");

        return await _repo.CambiarContrasenaAsync(id, request)
            ? NoContent()
            : BadRequest("La contraseña actual no es correcta.");
    }
}