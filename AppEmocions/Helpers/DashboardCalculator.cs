using System.Globalization;
using AppEmocions.Models.Dashboard;

namespace AppEmocions.Helpers;

public record KpiResultado(string ValorTexto, string VariacionTexto, bool MostrarVariacion, int EstadoVariacion);

public static class DashboardCalculator
{
    private static readonly CultureInfo Cultura = new("ca-ES");
    private static readonly bool[] MayorEsMejor = { true, true, true, true, false, false };

    public static KpiResultado[] CalcularKpis(ResumenDto? actual, ResumenDto? previo)
    {
        double?[] a = ExtraerMetricas(actual);
        double?[] p = ExtraerMetricas(previo);
        Func<double, string>[] formatos = { v => v.ToString("N0", Cultura),
            v => v.ToString("N0", Cultura),
            v => v.ToString("0.0", Cultura),
            v => v.ToString("0", Cultura) + "%",
            v => v.ToString("0.0", Cultura) + " s",
            v => v.ToString("0.0", Cultura) + "%" };
        var res = new KpiResultado[6];
        for (int i = 0; i < 6; i++)
        {
            string val = a[i] is double v ? formatos[i](v) : "-";
            bool relativa = i <= 2;
            if (a[i] == null || p[i] == null || (relativa && p[i]!.Value == 0))
            {
                res[i] = new KpiResultado(val, "", false, 0);
                continue;
            }
            double delta = relativa ? (a[i]!.Value - p[i]!.Value) / p[i]!.Value * 100 : a[i]!.Value - p[i]!.Value;
            string unidad = relativa ? "%" : i == 4 ? " s" : " pp";
            if (Math.Round(Math.Abs(delta), 1) == 0) res[i] = new KpiResultado(val, "= 0" + unidad, true, 0);
            else
            {
                bool sube = delta > 0;
                bool bueno = sube == MayorEsMejor[i];
                string txt = (sube ? "▲ " : "▼ ") + Math.Abs(delta).ToString("0.0", Cultura) + unidad;
                res[i] = new KpiResultado(val, txt, true, bueno ? 1 : -1);
            }
        }
        return res;
    }

    private static double?[] ExtraerMetricas(ResumenDto? r)
    {
        if (r == null) return new double?[6];
        double encertables = r.Aciertos + r.Errores;
        return new double?[] { r.ParticipantesActivos, r.Sesiones, r.PuntuacionMedia, encertables > 0 ? (double?)(r.Aciertos * 100 / encertables) : null, r.RespMediaMs / 1000, r.Sesiones > 0 ? (double?)(r.Abandonadas * 100 / r.Sesiones) : null };
    }

    public static List<(string Etiqueta, float Valor)> SerieSemanas(List<SemanaDto> semanas) => semanas.Where(s => s.Aciertos + s.Errores > 0).OrderBy(s => s.Semana).Select(s => ($"Set. {s.Semana}", (float)(s.Aciertos * 100 / (s.Aciertos + s.Errores)))).ToList();

    public static List<(string Nivel, float Acierto, float Error)> SerieNiveles(List<NivelDto> niveles)
    {
        if (niveles.Count == 0) return new();
        string[] nombres = { "La Plaça", "El Parc", "Plaça Gran", "El Jardí" };
        var datos = new List<(string, float, float)>();
        for (int n = 1; n <= 4; n++)
        {
            var d = niveles.FirstOrDefault(x => x.Nivel == n);
            float acierto = 0, error = 0;
            if (d != null)
            {
                double total = d.Aciertos + d.Errores;
                if (total > 0)
                {
                    acierto = (float)(d.Aciertos * 100 / total);
                    error = (float)(d.Errores * 100 / total);
                }
                else if (d.Registros > 0) { acierto = (float)(d.Completados * 100 / d.Registros); }
            }
            datos.Add((nombres[n - 1], acierto, error));
        }
        return datos;
    }
}
