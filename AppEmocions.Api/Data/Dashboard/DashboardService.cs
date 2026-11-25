namespace AppEmocions.Api.Data.Dashboard;

public sealed class DashboardService
{
    private readonly DashboardRepository _repo;

    private const int MinSesionesInterrumpidas = 2;
    private const double CaidaAciertosPuntos = 10;
    private const int MinIntentosParaComparar = 5;
    private const double FactorTiempoRespuesta = 1.25;

    public DashboardService(DashboardRepository repo) => _repo = repo;

    public static FiltroDashboard CrearFiltro(DateTime? desde, DateTime? hasta, string? grupo)
    {
        DateTime d = (desde ?? new DateTime(2000, 1, 1)).Date;
        DateTime h = (hasta ?? new DateTime(2999, 12, 31)).Date.AddDays(1);
        string? g = string.IsNullOrWhiteSpace(grupo) ? null : grupo.Trim();
        return new FiltroDashboard(d, h, g);
    }

    public Task<ResumenDashboardDto> ObtenerResumenAsync(DateTime? desde, DateTime? hasta, string? grupo)
        => _repo.ObtenerResumenAsync(CrearFiltro(desde, hasta, grupo));

    public Task<IEnumerable<SemanaEvolucionDto>> ObtenerSemanasAsync(DateTime? desde, DateTime? hasta, string? grupo)
        => _repo.ObtenerSemanasAsync(CrearFiltro(desde, hasta, grupo));

    public Task<IEnumerable<NivelEstadisticaDto>> ObtenerNivelesAsync(DateTime? desde, DateTime? hasta, string? grupo)
        => _repo.ObtenerNivelesAsync(CrearFiltro(desde, hasta, grupo));

    public Task<IEnumerable<string>> ObtenerGruposAsync()
        => _repo.ObtenerGruposAsync();

    public Task<IEnumerable<ComparativaGrupoDto>> ObtenerComparativaGruposAsync(DateTime? desde, DateTime? hasta, string? grupo)
        => _repo.ObtenerComparativaGruposAsync(CrearFiltro(desde, hasta, grupo));

    public Task<IEnumerable<ComparativaNivelGrupoDto>> ObtenerComparativaNivelesAsync(DateTime? desde, DateTime? hasta, string? grupo)
        => _repo.ObtenerComparativaNivelesAsync(CrearFiltro(desde, hasta, grupo));

    public Task<IEnumerable<AbandonoNivelGrupoDto>> ObtenerAbandonoPorNivelAsync(DateTime? desde, DateTime? hasta, string? grupo)
        => _repo.ObtenerAbandonoPorNivelAsync(CrearFiltro(desde, hasta, grupo));

    public async Task<IEnumerable<NinoAtencionDto>> ObtenerAtencionAsync(
        DateTime? desde, DateTime? hasta, string? grupo, int limite = 3)
    {
        var filas = (await _repo.ObtenerMetricasAtencionAsync(CrearFiltro(desde, hasta, grupo))).ToList();

        var tiempos = filas.Where(f => f.RespMs.HasValue).Select(f => f.RespMs!.Value).ToList();
        double mediaGrupo = tiempos.Count > 0 ? tiempos.Average() : 0;

        var candidatos = new List<(string Codigo, string Motivo, string Estado, int Prioridad)>();

        foreach (var f in filas)
        {
            if (f.Interrumpidas >= MinSesionesInterrumpidas)
            {
                candidatos.Add((f.Codigo, $"{f.Interrumpidas} sessions interrompudes", "Revisar", 3));
                continue;
            }

            if (f.TotRec >= MinIntentosParaComparar && f.TotAnt >= MinIntentosParaComparar)
            {
                double rec = f.AcRec * 100.0 / f.TotRec;
                double ant = f.AcAnt * 100.0 / f.TotAnt;
                if (ant - rec >= CaidaAciertosPuntos)
                {
                    candidatos.Add((f.Codigo, "Baixada d'encerts recent", "Revisar", 2));
                    continue;
                }
            }

            if (f.RespMs.HasValue && mediaGrupo > 0 && f.RespMs.Value > mediaGrupo * FactorTiempoRespuesta)
            {
                candidatos.Add((f.Codigo, "Més temps de resposta", "Observar", 1));
            }
        }

        var query = candidatos
            .OrderByDescending(c => c.Prioridad)
            .ThenBy(c => c.Codigo);

        return (limite > 0 ? query.Take(limite) : query)
            .Select(c => new NinoAtencionDto(c.Codigo, c.Motivo, c.Estado));
    }
}