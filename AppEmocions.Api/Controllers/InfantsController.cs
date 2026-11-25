using AppEmocions.Api.Data.Dashboard;
using AppEmocions.Api.Data.Infants;
using Microsoft.AspNetCore.Mvc;

namespace AppEmocions.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class InfantsController : ControllerBase
{
    private readonly InfantsRepository _repo;

    public InfantsController(InfantsRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<IActionResult> ObtenerListado(
        [FromQuery] DateTime? desde, [FromQuery] DateTime? hasta,
        [FromQuery] string? grupo, [FromQuery] string? q, [FromQuery] bool soloActivos = true)
    {
        var filtro = DashboardService.CrearFiltro(desde, hasta, grupo);
        return Ok(await _repo.ObtenerListadoAsync(filtro.Desde, filtro.HastaExclusivo, filtro.Grupo, q, soloActivos));
    }

    [HttpGet("{codigo}")]
    public async Task<IActionResult> ObtenerDetalle(string codigo, [FromQuery] DateTime? desde, [FromQuery] DateTime? hasta)
    {
        var filtro = DashboardService.CrearFiltro(desde, hasta, null);
        var detalle = await _repo.ObtenerDetalleAsync(codigo, filtro.Desde, filtro.HastaExclusivo);
        return detalle is null ? NotFound("No se ha encontrado el participante.") : Ok(detalle);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearInfantRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre) || string.IsNullOrWhiteSpace(request.Grupo))
            return BadRequest("El nombre y el grupo son obligatorios.");

        var (creado, id, codigo) = await _repo.CrearConSpAsync(request);
        if (!creado)
            return Conflict("Ya existe un participante con ese nombre.");

        return Ok(new { participante_id = id, codigo });
    }

    [HttpPut("{id:int}/docente")]
    public async Task<IActionResult> AsignarDocente(int id, [FromBody] AsignarDocenteRequest request)
    {
        bool ok = await _repo.AsignarDocenteAsync(id, request);
        return ok ? NoContent() : NotFound("No se ha encontrado el participante.");
    }
}