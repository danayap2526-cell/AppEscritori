using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AppEmocions.Data;

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

// =====================================================
// DTOs DEL DASHBOARD Y COMPARATIVAS
// =====================================================

public class ResumenDto
{
    public double ParticipantesActivos { get; set; }
    public double Sesiones { get; set; }
    [JsonPropertyName("sesionesCompletadas")] public double Completadas { get; set; }
    [JsonPropertyName("sesionesAbandonadas")] public double Abandonadas { get; set; }
    public double? PuntuacionMedia { get; set; }
    public double? RespMediaMs { get; set; }
    public double Aciertos { get; set; }
    public double Errores { get; set; }
}

public class SemanaDto
{
    public int Semana { get; set; }
    public string? Grupo { get; set; }
    public double Aciertos { get; set; }
    public double Errores { get; set; }
    public double TasaAcierto { get; set; }
}

public class NivelDto
{
    public int Nivel { get; set; }
    public double Aciertos { get; set; }
    public double Errores { get; set; }
    public double Registros { get; set; }
    public double Completados { get; set; }
}

public class AtencionDto
{
    public string Codigo { get; set; } = "";
    public string Motivo { get; set; } = "";
    public string Estado { get; set; } = "";
}

public class ComparativaGrupoDto
{
    public string Grupo { get; set; } = "";
    public int TotalInfants { get; set; }
    public int Sesiones { get; set; }
    public double TasaAcierto { get; set; }
    public double RespMediaMs { get; set; }
    public double TasaAbandono { get; set; }
    public double TasaFinalizacion { get; set; }
}

public class ComparativaNivelGrupoDto
{
    public string Grupo { get; set; } = "";
    public byte Nivel { get; set; }
    public double TasaAcierto { get; set; }
}

public class AbandonoNivelGrupoDto
{
    public byte Nivel { get; set; }
    public string Grupo { get; set; } = "";
    public int Iniciadas { get; set; }
    public int Completadas { get; set; }
    public int Descansar { get; set; }
    public int Sortida { get; set; }
    public int SegonPla { get; set; }
}

// =====================================================
// DTOs DE INFANTS
// =====================================================

public class InfantListadoDto
{
    public int ParticipanteId { get; set; }
    public string Codigo { get; set; } = "";
    public string? Nombre { get; set; }
    public string Grupo { get; set; } = "";
    public int? DocenteId { get; set; }
    public string? NombreDocente { get; set; }
    public bool Activo { get; set; }
    public int Sessions { get; set; }
    public int DiesActius { get; set; }
    public DateTime? UltimaSessio { get; set; }
    public double TaxaEncert { get; set; }
    public double TempsRespostaMs { get; set; }
    public int Abandonaments { get; set; }
}

public class InfantDetalleCabeceraDto
{
    public int ParticipanteId { get; set; }
    public string Codigo { get; set; } = "";
    public string? Nombre { get; set; }
    public string Grupo { get; set; } = "";
    public int? DocenteId { get; set; }
    public string? NombreDocente { get; set; }
    public bool Activo { get; set; }
    public DateTime? PrimeraSessio { get; set; }
    public DateTime? UltimaSessio { get; set; }
    public int Sessions { get; set; }
    public double TaxaEncert { get; set; }
    public double TempsRespostaMs { get; set; }
}

public class InfantNivelDto
{
    public byte Nivel { get; set; }
    public int Registros { get; set; }
    public int Completados { get; set; }
    public int Aciertos { get; set; }
    public int Errores { get; set; }
    public double RespMediaMs { get; set; }
}

public class InfantSesionDto
{
    public string SesionId { get; set; } = "";
    public string NombreJuego { get; set; } = "";
    public DateTime IniciadaEn { get; set; }
    public int? DuracionS { get; set; }
    public decimal? Puntuacion { get; set; }
    public byte? NivelAlcanzado { get; set; }
    public bool Completada { get; set; }
    public bool Abandonada { get; set; }
    public int Aciertos { get; set; }
    public int Errores { get; set; }
}

public record InfantDetalleCompletoDto(
    InfantDetalleCabeceraDto Cabecera,
    List<InfantNivelDto> PorNivel,
    List<InfantSesionDto> Sesiones
);

public record CrearInfantRequest(string Nombre, string Grupo, int? DocenteId, string? ContrasenaHash = null);

public class CrearInfantResponse
{
    [JsonPropertyName("participante_id")] public int ParticipanteId { get; set; }
    public string Codigo { get; set; } = "";
}

public record AsignarDocenteRequest(int? DocenteId, string? Grupo = null);

// =====================================================
// DTOs DE JOCS, USUARIS Y DADES
// =====================================================

public class JuegoDto
{
    public int JuegoId { get; set; }
    public string Nombre { get; set; } = "";
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
}

public record GuardarJuegoRequest(string Nombre, string? Descripcion, bool Activo);

public class DocenteDto
{
    public int DocenteId { get; set; }
    public string Usuario { get; set; } = "";
    public string NombreVisible { get; set; } = "";
    public string? Email { get; set; }
    public string Rol { get; set; } = "";
    public bool Activo { get; set; }
    public DateTime? UltimoAcceso { get; set; }
    public List<string> Grupos { get; set; } = new();
}

public record CrearDocenteRequest(
    string Usuario,
    string NombreVisible,
    string? Email,
    string Contrasena,
    string Rol,
    List<string> Grupos
);

public record CambiarContrasenaRequest(string ContrasenaActual, string ContrasenaNueva);

public class SesionImportadaDto
{
    public string SesionId { get; set; } = "";
    public string CodigoInfant { get; set; } = "";
    public DateTime Data { get; set; }
    public string Joc { get; set; } = "";
    public bool Simulada { get; set; }
    public string Estat { get; set; } = "";
}

public record IncidenciaCoherenciaDto(string Tipo, string Referencia, string Detalle);

public record ResumenCalidadDatosDto(
    int TotalSesiones,
    int SesionesSinIncidencias,
    double PorcentajeCalidad,
    List<IncidenciaCoherenciaDto> Incidencias
);