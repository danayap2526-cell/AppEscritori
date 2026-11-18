
using AppEmocions.Api.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AppEmocions.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBaseDatos
{
    public DashboardController(Db db) : base(db)
    {
    }

    // GET: api/dashboard/resumen
    [HttpGet("resumen")]
    public IActionResult Resumen()
    {
        const string sql = @"
            SELECT
                (SELECT COUNT(*)
                 FROM dbo.participante
                 WHERE activo = 1) AS participantes_activos,

                COUNT(*) AS sesiones,

                SUM(CASE WHEN completada = 1
                    THEN 1 ELSE 0 END) AS sesiones_completadas,

                SUM(CASE WHEN abandonada = 1
                    THEN 1 ELSE 0 END) AS sesiones_abandonadas

            FROM dbo.sesion
            WHERE simulada = 0;";

        var tabla = _db.Consultar(sql);
        return Ok(Convertir(tabla).FirstOrDefault());
    }

    // GET: api/dashboard/tasa-acierto
    [HttpGet("tasa-acierto")]
    public IActionResult TasaAcierto()
    {
        const string sql = @"
            SELECT
                COALESCE(SUM(ns.aciertos), 0) AS aciertos,
                COALESCE(SUM(ns.errores), 0) AS errores,

                CAST(
                    COALESCE(
                        SUM(ns.aciertos) * 100.0 /
                        NULLIF(SUM(ns.aciertos + ns.errores), 0),
                        0
                    ) AS DECIMAL(5,2)
                ) AS porcentaje_acierto

            FROM dbo.nivel_sesion ns
            INNER JOIN dbo.sesion s
                ON s.sesion_id = ns.sesion_id

            WHERE s.simulada = 0
              AND ns.nivel IN (1, 2);";

        var tabla = _db.Consultar(sql);
        return Ok(Convertir(tabla).FirstOrDefault());
    }

    // GET: api/dashboard/emociones
    [HttpGet("emociones")]
    public IActionResult Emociones()
    {
        const string sql = @"
            SELECT
                COALESCE(SUM(ns.err_alegria), 0) AS errores_alegria,
                COALESCE(SUM(ns.err_tristeza), 0) AS errores_tristeza,
                COALESCE(SUM(ns.err_ira), 0) AS errores_ira,
                COALESCE(SUM(ns.err_miedo), 0) AS errores_miedo

            FROM dbo.nivel_sesion ns
            INNER JOIN dbo.sesion s
                ON s.sesion_id = ns.sesion_id

            WHERE s.simulada = 0
              AND ns.nivel IN (1, 2);";

        var tabla = _db.Consultar(sql);
        return Ok(Convertir(tabla).FirstOrDefault());
    }

    // GET: api/dashboard/acciones-ayuda
    [HttpGet("acciones-ayuda")]
    public IActionResult AccionesAyuda()
    {
        const string sql = @"
            SELECT
                COALESCE(SUM(ns.n_abrazar), 0) AS abrazar,
                COALESCE(SUM(ns.n_compartir), 0) AS compartir,
                COALESCE(SUM(ns.n_avisar_adulto), 0) AS avisar_adulto

            FROM dbo.nivel_sesion ns
            INNER JOIN dbo.sesion s
                ON s.sesion_id = ns.sesion_id

            WHERE s.simulada = 0
              AND ns.nivel = 3;";

        var tabla = _db.Consultar(sql);
        return Ok(Convertir(tabla).FirstOrDefault());
    }

    // GET: api/dashboard/respiracion
    [HttpGet("respiracion")]
    public IActionResult Respiracion()
    {
        const string sql = @"
            SELECT
                COALESCE(SUM(ns.ciclos_completados), 0)
                    AS ciclos_completados

            FROM dbo.nivel_sesion ns
            INNER JOIN dbo.sesion s
                ON s.sesion_id = ns.sesion_id

            WHERE s.simulada = 0
              AND ns.nivel = 4;";

        var tabla = _db.Consultar(sql);
        return Ok(Convertir(tabla).FirstOrDefault());
    }
}
