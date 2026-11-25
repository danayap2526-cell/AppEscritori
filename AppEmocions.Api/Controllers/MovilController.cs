using AppEmocions.Api.Data.Movil;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace AppEmocions.Api.Controllers;

[ApiController]
[Route("api/movil")]
public sealed class MovilController : ControllerBase
{
    private readonly MovilRepository _repo;

    public MovilController(MovilRepository repo) => _repo = repo;

    [HttpPost("sesion")]
    public async Task<IActionResult> GuardarSesion([FromBody] SesionMovilDto d)
    {
        if (string.IsNullOrWhiteSpace(d.Codigo) || d.Codigo.Length > 100)
            return BadRequest("Usuario obligatorio (máx. 100 caracteres).");
        if (d.Nivel < 1 || d.Nivel > 4)
            return BadRequest("Nivel debe estar entre 1 y 4.");
        if (d.DuracionS < 0 || d.Intentos < 0 || d.Errores < 0 || d.Ayudas < 0 || d.Aciertos < 0)
            return BadRequest("Los contadores no pueden ser negativos.");

        try
        {
            var sesionId = await _repo.GuardarSesionAsync(d);
            return Ok(new { sesion_id = sesionId });
        }
        catch (SqlException ex)
        {
            return StatusCode(500, "Error al guardar en la base de datos: " + ex.Message);
        }
    }

    [HttpGet("sesiones/{usuario}")]
    public async Task<IActionResult> SesionesDe(string usuario)
        => Ok(await _repo.ObtenerSesionesDeAsync(usuario));

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AccesoMovilDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.PasswordHash))
            return BadRequest();

        bool ok = await _repo.ValidarLoginAsync(dto.Username, dto.PasswordHash);
        return ok ? Ok() : Unauthorized();
    }

    [HttpPost("registro")]
    public async Task<IActionResult> Registro([FromBody] AccesoMovilDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username) || dto.Username.Length > 100 || string.IsNullOrWhiteSpace(dto.PasswordHash))
            return BadRequest();

        var (creado, codigo) = await _repo.RegistrarParticipanteAsync(dto.Username, dto.PasswordHash);
        return creado ? Ok(new { codigo }) : Conflict();
    }
}