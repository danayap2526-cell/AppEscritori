using AppEmocions.Api.Dtos.Movil;
using AppEmocions.Api.Data;
using AppEmocions.Api.Dtos.Dades;
using Microsoft.AspNetCore.Mvc;

namespace AppEmocions.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class DadesController : ControllerBase
{
    private readonly DadesRepository _repo;
    private readonly MovilRepository _movilRepo;

    public DadesController(DadesRepository repo, MovilRepository movilRepo)
    {
        _repo = repo;
        _movilRepo = movilRepo;
    }

    [HttpGet("sesiones")]
    public async Task<IActionResult> ObtenerSesiones([FromQuery] bool incluirSimuladas = false)
        => Ok(await _repo.ObtenerSesionesAsync(incluirSimuladas));

    [HttpGet("coherencia")]
    public async Task<IActionResult> ObtenerCoherencia()
        => Ok(await _repo.ObtenerControlCoherenciaAsync());

    [HttpPost("importar")]
    public async Task<IActionResult> ImportarSesiones([FromBody] List<SesionMovilDto> sesiones)
    {
        int importadas = 0;
        int ignoradas = 0;

        foreach (var sesion in sesiones)
        {
            try
            {
                await _movilRepo.GuardarSesionAsync(sesion);
                importadas++;
            }
            catch
            {
                ignoradas++;
            }
        }

        return Ok(new { importadas, ignoradas });
    }
}