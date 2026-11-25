using AppEmocions.Models.Dades;
using AppEmocions.Models.Dashboard;
using AppEmocions.Models.Infants;
using AppEmocions.Models.Jocs;
using AppEmocions.Models.Usuaris;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AppEmocions.Services;

public static class ApiClient
{
    private static readonly HttpClient _http = new()
    {
        BaseAddress = new Uri("http://localhost:5102/"),
        Timeout = TimeSpan.FromSeconds(10)
    };

    private static readonly JsonSerializerOptions _jsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // =====================================================
    // ESTADO / CONEXIÓN
    // =====================================================

    public static async Task<(bool ok, string mensaje)> ProbarAsync(CancellationToken ct = default)
    {
        try
        {
            var resp = await _http.GetAsync("api/dashboard/grupos", ct);
            return resp.IsSuccessStatusCode
                ? (true, "Conectado correctamente a AppEmocions.Api")
                : (false, $"Error HTTP {(int)resp.StatusCode}: {resp.ReasonPhrase}");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    // =====================================================
    // DASHBOARD Y COMPARATIVAS
    // =====================================================

    private static string Filtros(DateTime desde, DateTime hasta, string? grupo)
    {
        var inv = CultureInfo.InvariantCulture;
        string q = $"desde={desde.ToString("yyyy-MM-dd", inv)}" +
                   $"&hasta={hasta.ToString("yyyy-MM-dd", inv)}";

        if (!string.IsNullOrWhiteSpace(grupo))
            q += "&grupo=" + Uri.EscapeDataString(grupo);

        return q;
    }

    public static async Task<List<string>> ListarGruposAsync(CancellationToken ct = default) =>
        await _http.GetFromJsonAsync<List<string>>("api/dashboard/grupos", _jsonOpts, ct) ?? new();

    public static Task<ResumenDto?> ResumenAsync(
        DateTime desde, DateTime hasta, string? grupo, CancellationToken ct = default) =>
        _http.GetFromJsonAsync<ResumenDto>(
            $"api/dashboard/resumen?{Filtros(desde, hasta, grupo)}", _jsonOpts, ct);

    public static Task<List<SemanaDto>?> SemanasAsync(
        DateTime desde, DateTime hasta, string? grupo, CancellationToken ct = default) =>
        _http.GetFromJsonAsync<List<SemanaDto>>(
            $"api/dashboard/semanas?{Filtros(desde, hasta, grupo)}", _jsonOpts, ct);

    public static Task<List<NivelDto>?> NivelesAsync(
        DateTime desde, DateTime hasta, string? grupo, CancellationToken ct = default) =>
        _http.GetFromJsonAsync<List<NivelDto>>(
            $"api/dashboard/niveles?{Filtros(desde, hasta, grupo)}", _jsonOpts, ct);

    // Sobrecarga compatible con llamadas de 4 argumentos: (desde, hasta, grupo, ct)
    public static Task<List<AtencionDto>?> AtencionAsync(
        DateTime desde, DateTime hasta, string? grupo, CancellationToken ct) =>
        AtencionAsync(desde, hasta, grupo, limite: 3, ct);

    public static Task<List<AtencionDto>?> AtencionAsync(
        DateTime desde, DateTime hasta, string? grupo, int limite = 3, CancellationToken ct = default) =>
        _http.GetFromJsonAsync<List<AtencionDto>>(
            $"api/dashboard/atencion?{Filtros(desde, hasta, grupo)}&limite={limite}", _jsonOpts, ct);

    public static async Task<List<ComparativaGrupoDto>> ComparativaGruposAsync(
        DateTime desde, DateTime hasta, string? grupo = null, CancellationToken ct = default) =>
        await _http.GetFromJsonAsync<List<ComparativaGrupoDto>>(
            $"api/dashboard/comparativa-grupos?{Filtros(desde, hasta, grupo)}", _jsonOpts, ct) ?? new();

    public static async Task<List<ComparativaNivelGrupoDto>> ComparativaNivelesAsync(
        DateTime desde, DateTime hasta, string? grupo = null, CancellationToken ct = default) =>
        await _http.GetFromJsonAsync<List<ComparativaNivelGrupoDto>>(
            $"api/dashboard/comparativa-niveles?{Filtros(desde, hasta, grupo)}", _jsonOpts, ct) ?? new();

    public static async Task<List<AbandonoNivelGrupoDto>> AbandonoPorNivelAsync(
        DateTime desde, DateTime hasta, string? grupo = null, CancellationToken ct = default) =>
        await _http.GetFromJsonAsync<List<AbandonoNivelGrupoDto>>(
            $"api/dashboard/abandono?{Filtros(desde, hasta, grupo)}", _jsonOpts, ct) ?? new();

    // =====================================================
    // INFANTS (PARTICIPANTES)
    // =====================================================

    public static async Task<List<InfantListadoDto>> ListarInfantsAsync(
        DateTime desde, DateTime hasta, string? grupo = null, string? busqueda = null,
        bool soloActivos = true, CancellationToken ct = default)
    {
        string url = $"api/infants?{Filtros(desde, hasta, grupo)}&soloActivos={soloActivos}";
        if (!string.IsNullOrWhiteSpace(busqueda))
            url += "&q=" + Uri.EscapeDataString(busqueda.Trim());

        return await _http.GetFromJsonAsync<List<InfantListadoDto>>(url, _jsonOpts, ct) ?? new();
    }

    public static Task<InfantDetalleCompletoDto?> ObtenerDetalleInfantAsync(
        string codigo, DateTime desde, DateTime hasta, CancellationToken ct = default) =>
        _http.GetFromJsonAsync<InfantDetalleCompletoDto>(
            $"api/infants/{Uri.EscapeDataString(codigo)}?{Filtros(desde, hasta, null)}", _jsonOpts, ct);

    public static async Task<CrearInfantResponse> CrearInfantAsync(
        CrearInfantRequest req, CancellationToken ct = default)
    {
        var resp = await _http.PostAsJsonAsync("api/infants", req, ct);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<CrearInfantResponse>(_jsonOpts, ct)
               ?? throw new InvalidOperationException("Respuesta vacía al crear participante.");
    }

    public static async Task AsignarDocenteInfantAsync(
        int participanteId, int? docenteId, string? grupo = null, CancellationToken ct = default)
    {
        var resp = await _http.PutAsJsonAsync(
            $"api/infants/{participanteId}/docente",
            new AsignarDocenteRequest(docenteId, grupo), ct);
        resp.EnsureSuccessStatusCode();
    }

    // =====================================================
    // JOCS (JUEGOS)
    // =====================================================

    public static async Task<List<JuegoDto>> ListarJocsAsync(CancellationToken ct = default) =>
        await _http.GetFromJsonAsync<List<JuegoDto>>("api/jocs", _jsonOpts, ct) ?? new();

    public static async Task CrearJocAsync(GuardarJuegoRequest req, CancellationToken ct = default)
    {
        var resp = await _http.PostAsJsonAsync("api/jocs", req, ct);
        resp.EnsureSuccessStatusCode();
    }

    public static async Task ActualizarJocAsync(int id, GuardarJuegoRequest req, CancellationToken ct = default)
    {
        var resp = await _http.PutAsJsonAsync($"api/jocs/{id}", req, ct);
        resp.EnsureSuccessStatusCode();
    }

    public static async Task CambiarEstadoJocAsync(int id, bool activo, CancellationToken ct = default)
    {
        var resp = await _http.PatchAsJsonAsync($"api/jocs/{id}/activo", activo, ct);
        resp.EnsureSuccessStatusCode();
    }

    // =====================================================
    // USUARIS (DOCENTES)
    // =====================================================

    public static async Task<List<DocenteDto>> ListarDocentesAsync(CancellationToken ct = default) =>
        await _http.GetFromJsonAsync<List<DocenteDto>>("api/usuaris/docentes", _jsonOpts, ct) ?? new();

    public static async Task CrearDocenteAsync(CrearDocenteRequest req, CancellationToken ct = default)
    {
        var resp = await _http.PostAsJsonAsync("api/usuaris/docentes", req, ct);
        resp.EnsureSuccessStatusCode();
    }

    public static async Task CambiarEstadoDocenteAsync(int id, bool activo, CancellationToken ct = default)
    {
        var resp = await _http.PatchAsJsonAsync($"api/usuaris/docentes/{id}/activo", activo, ct);
        resp.EnsureSuccessStatusCode();
    }

    public static async Task<bool> CambiarContrasenaDocenteAsync(
        int id, string contrasenaActual, string contrasenaNueva, CancellationToken ct = default)
    {
        var resp = await _http.PostAsJsonAsync(
            $"api/usuaris/docentes/{id}/contrasena",
            new CambiarContrasenaRequest(contrasenaActual, contrasenaNueva), ct);
        return resp.IsSuccessStatusCode;
    }

    // =====================================================
    // DADES (SESIONES E IMPORTACIÓN)
    // =====================================================

    public static async Task<List<SesionImportadaDto>> ListarSesionesDadesAsync(
        bool incluirSimuladas = false, CancellationToken ct = default) =>
        await _http.GetFromJsonAsync<List<SesionImportadaDto>>(
            $"api/dades/sesiones?incluirSimuladas={incluirSimuladas}", _jsonOpts, ct) ?? new();

    public static Task<ResumenCalidadDatosDto?> ObtenerCoherenciaDatosAsync(CancellationToken ct = default) =>
        _http.GetFromJsonAsync<ResumenCalidadDatosDto>("api/dades/coherencia", _jsonOpts, ct);
}


