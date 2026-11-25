using AppEmocions.Api.Data;
using AppEmocions.Api.Dtos.Dashboard;
using AppEmocions.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AppEmocions.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class DashboardController : ControllerBase
{
    private readonly DashboardService _service;

    public DashboardController(DashboardService service) => _service = service;

    [HttpGet("resumen")]
    public async Task<IActionResult> Resumen([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta, [FromQuery] string? grupo)
        => Ok(await _service.ObtenerResumenAsync(desde, hasta, grupo));

    [HttpGet("semanas")]
    public async Task<IActionResult> Semanas([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta, [FromQuery] string? grupo)
        => Ok(await _service.ObtenerSemanasAsync(desde, hasta, grupo));

    [HttpGet("niveles")]
    public async Task<IActionResult> Niveles([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta, [FromQuery] string? grupo)
        => Ok(await _service.ObtenerNivelesAsync(desde, hasta, grupo));

    [HttpGet("atencion")]
    public async Task<IActionResult> Atencion([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta, [FromQuery] string? grupo, [FromQuery] int limite = 3)
        => Ok(await _service.ObtenerAtencionAsync(desde, hasta, grupo, limite));

    [HttpGet("grupos")]
    public async Task<IActionResult> Grupos()
        => Ok(await _service.ObtenerGruposAsync());

    [HttpGet("comparativa-grupos")]
    public async Task<IActionResult> ComparativaGrupos([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta, [FromQuery] string? grupo)
        => Ok(await _service.ObtenerComparativaGruposAsync(desde, hasta, grupo));

    [HttpGet("comparativa-niveles")]
    public async Task<IActionResult> ComparativaNiveles([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta, [FromQuery] string? grupo)
        => Ok(await _service.ObtenerComparativaNivelesAsync(desde, hasta, grupo));

    [HttpGet("abandono")]
    public async Task<IActionResult> Abandono([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta, [FromQuery] string? grupo)
        => Ok(await _service.ObtenerAbandonoPorNivelAsync(desde, hasta, grupo));
}