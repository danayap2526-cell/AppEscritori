using AppEmocions.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace AppEmocions.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NivelesController : ControllerBaseDatos
{
    public NivelesController(Db db) : base(db) { }

    [HttpGet("{sesionId}")]
    public IActionResult ObtenerPorSesion(string sesionId)
    {
        const string sql = @"
            SELECT
                ns.sesion_id,
                ns.nivel,
                ns.completado,
                ns.motiu_abandonament,
                ns.duracion_ms,
                ns.intentos,
                ns.errores,
                ns.ayudas,
                ns.resp_media_ms,
                ns.err_alegria,
                ns.err_tristeza,
                ns.err_ira,
                ns.err_miedo,
                ns.n_abrazar,
                ns.n_compartir,
                ns.n_avisar_adulto,
                ns.ciclos_completados,
                ns.aciertos
            FROM dbo.nivel_sesion ns
            INNER JOIN dbo.sesion s
                ON s.sesion_id = ns.sesion_id
            WHERE ns.sesion_id = @sesionId
              AND s.simulada = 0
            ORDER BY ns.nivel;";

        return Ok(Convertir(_db.Consultar(
            sql,
            new SqlParameter("@sesionId", sesionId)
        )));
    }
}