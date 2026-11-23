using AppEmocions.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq;

namespace AppEmocions.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBaseDatos
{
    public DashboardController(Db db) : base(db)
    {
    }

    // =====================================================
    // Umbrales de "infants que mereixen atenció"
    // =====================================================
    private const int MinSesionesInterrumpidas = 2;
    private const double CaidaAciertosPuntos = 10;   // puntos porcentuales
    private const int MinIntentosParaComparar = 5;   // mínimo de aciertos+errores por tramo
    private const double FactorTiempoRespuesta = 1.25;

    // =====================================================
    // Sesiones reales (no simuladas) filtradas por fechas y grupo.
    // Parámetros: @desde (incl.), @hasta (excl.), @grupo (NULL = todos)
    // =====================================================
    private const string CteSesiones = @"
        WITH ses AS (
            SELECT
                s.sesion_id,
                s.participante_id,
                p.codigo,
                s.iniciada_en,
                s.puntuacion,
                s.resp_media_ms,
                s.completada,
                s.abandonada,
                COALESCE(s.aciertos, 0) AS aciertos,
                COALESCE(s.errores, 0)  AS errores
            FROM dbo.sesion s
            INNER JOIN dbo.participante p
                ON p.participante_id = s.participante_id
            WHERE s.simulada = 0
              AND s.iniciada_en >= @desde
              AND s.iniciada_en <  @hasta
              AND (@grupo IS NULL OR p.grupo = @grupo)
        )";

    private static SqlParameter[] Filtros(DateTime? desde, DateTime? hasta, string? grupo)
    {
        DateTime d = (desde ?? new DateTime(2000, 1, 1)).Date;
        DateTime h = (hasta ?? new DateTime(2999, 12, 31)).Date.AddDays(1); // exclusivo

        return new[]
        {
            new SqlParameter("@desde", SqlDbType.DateTime) { Value = d },
            new SqlParameter("@hasta", SqlDbType.DateTime) { Value = h },
            new SqlParameter("@grupo", SqlDbType.NVarChar, 50)
            {
                Value = string.IsNullOrWhiteSpace(grupo) ? DBNull.Value : (object)grupo
            }
        };
    }

    private static double Num(object? o) => o == null ? 0 : Convert.ToDouble(o);

    // GET: api/dashboard/resumen?desde=&hasta=&grupo=
    [HttpGet("resumen")]
    public IActionResult Resumen(
        [FromQuery] DateTime? desde, [FromQuery] DateTime? hasta, [FromQuery] string? grupo)
    {
        const string sql = CteSesiones + @"
            SELECT
                (SELECT COUNT(DISTINCT participante_id) FROM ses)
                    AS participantes_activos,
                (SELECT COUNT(*) FROM ses)
                    AS sesiones,
                (SELECT COUNT(*) FROM ses WHERE completada = 1)
                    AS sesiones_completadas,
                (SELECT COUNT(*) FROM ses WHERE abandonada = 1)
                    AS sesiones_abandonadas,
                (SELECT AVG(CAST(puntuacion AS FLOAT)) FROM ses)
                    AS puntuacion_media,
                (SELECT AVG(CAST(resp_media_ms AS FLOAT)) FROM ses
                  WHERE resp_media_ms IS NOT NULL)
                    AS resp_media_ms,
                (SELECT COALESCE(SUM(ns.aciertos), 0)
                   FROM dbo.nivel_sesion ns
                   INNER JOIN ses ON ses.sesion_id = ns.sesion_id
                  WHERE ns.nivel IN (1, 2))
                    AS aciertos,
                (SELECT COALESCE(SUM(ns.errores), 0)
                   FROM dbo.nivel_sesion ns
                   INNER JOIN ses ON ses.sesion_id = ns.sesion_id
                  WHERE ns.nivel IN (1, 2))
                    AS errores;";

        var tabla = _db.Consultar(sql, Filtros(desde, hasta, grupo));
        return Ok(Convertir(tabla).FirstOrDefault());
    }

    // GET: api/dashboard/semanas?desde=&hasta=&grupo=
    // Aciertos/errores (niveles 1 y 2) por semana, empezando en "desde".
    [HttpGet("semanas")]
    public IActionResult Semanas(
        [FromQuery] DateTime? desde, [FromQuery] DateTime? hasta, [FromQuery] string? grupo)
    {
        const string sql = CteSesiones + @"
            SELECT
                semana,
                COALESCE(SUM(aciertos), 0) AS aciertos,
                COALESCE(SUM(errores), 0)  AS errores
            FROM (
                SELECT
                    DATEDIFF(DAY, @desde, ses.iniciada_en) / 7 + 1 AS semana,
                    ns.aciertos,
                    ns.errores
                FROM dbo.nivel_sesion ns
                INNER JOIN ses ON ses.sesion_id = ns.sesion_id
                WHERE ns.nivel IN (1, 2)
            ) t
            GROUP BY semana
            ORDER BY semana;";

        return Ok(Convertir(_db.Consultar(sql, Filtros(desde, hasta, grupo))));
    }

    // GET: api/dashboard/niveles?desde=&hasta=&grupo=
    [HttpGet("niveles")]
    public IActionResult Niveles(
        [FromQuery] DateTime? desde, [FromQuery] DateTime? hasta, [FromQuery] string? grupo)
    {
        const string sql = CteSesiones + @"
            SELECT
                ns.nivel,
                COALESCE(SUM(ns.aciertos), 0) AS aciertos,
                COALESCE(SUM(ns.errores), 0)  AS errores,
                COUNT(*) AS registros,
                SUM(CASE WHEN ns.completado = 1 THEN 1 ELSE 0 END) AS completados
            FROM dbo.nivel_sesion ns
            INNER JOIN ses ON ses.sesion_id = ns.sesion_id
            GROUP BY ns.nivel
            ORDER BY ns.nivel;";

        return Ok(Convertir(_db.Consultar(sql, Filtros(desde, hasta, grupo))));
    }

    // GET: api/dashboard/atencion?desde=&hasta=&grupo=
    [HttpGet("atencion")]
    public IActionResult Atencion(
        [FromQuery] DateTime? desde, [FromQuery] DateTime? hasta, [FromQuery] string? grupo)
    {
        // Últimos 7 días del rango frente al resto
        const string sql = CteSesiones + @"
            SELECT
                codigo,
                SUM(CASE WHEN abandonada = 1 THEN 1 ELSE 0 END) AS interrumpidas,
                AVG(CAST(resp_media_ms AS FLOAT)) AS resp_ms,
                SUM(CASE WHEN iniciada_en >= DATEADD(DAY, -7, @hasta)
                         THEN aciertos ELSE 0 END) AS ac_rec,
                SUM(CASE WHEN iniciada_en >= DATEADD(DAY, -7, @hasta)
                         THEN aciertos + errores ELSE 0 END) AS tot_rec,
                SUM(CASE WHEN iniciada_en <  DATEADD(DAY, -7, @hasta)
                         THEN aciertos ELSE 0 END) AS ac_ant,
                SUM(CASE WHEN iniciada_en <  DATEADD(DAY, -7, @hasta)
                         THEN aciertos + errores ELSE 0 END) AS tot_ant
            FROM ses
            GROUP BY codigo;";

        var filas = Convertir(_db.Consultar(sql, Filtros(desde, hasta, grupo)));

        var tiempos = filas
            .Where(f => f["resp_ms"] != null)
            .Select(f => Num(f["resp_ms"]))
            .ToList();
        double mediaGrupo = tiempos.Count > 0 ? tiempos.Average() : 0;

        var candidatos = new List<(string Codigo, string Motivo, string Estado, int Prioridad)>();

        foreach (var f in filas)
        {
            string codigo = f["codigo"]?.ToString() ?? "";
            int interrumpidas = (int)Num(f["interrumpidas"]);
            double totRec = Num(f["tot_rec"]);
            double totAnt = Num(f["tot_ant"]);

            if (interrumpidas >= MinSesionesInterrumpidas)
            {
                candidatos.Add((codigo, $"{interrumpidas} sessions interrompudes", "Revisar", 3));
                continue;
            }

            if (totRec >= MinIntentosParaComparar && totAnt >= MinIntentosParaComparar)
            {
                double rec = Num(f["ac_rec"]) * 100 / totRec;
                double ant = Num(f["ac_ant"]) * 100 / totAnt;

                if (ant - rec >= CaidaAciertosPuntos)
                {
                    candidatos.Add((codigo, "Baixada d'encerts recent", "Revisar", 2));
                    continue;
                }
            }

            if (f["resp_ms"] != null && mediaGrupo > 0
                && Num(f["resp_ms"]) > mediaGrupo * FactorTiempoRespuesta)
            {
                candidatos.Add((codigo, "Més temps de resposta", "Observar", 1));
            }
        }

        var resultado = candidatos
            .OrderByDescending(c => c.Prioridad)
            .ThenBy(c => c.Codigo)
            .Take(3)
            .Select(c => new { codigo = c.Codigo, motivo = c.Motivo, estado = c.Estado });

        return Ok(resultado);
    }

    // =====================================================
    // Endpoints anteriores (sin cambios)
    // =====================================================

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