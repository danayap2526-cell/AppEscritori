using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace AppEmocions.Data
{
    public static class ApiClient
    {
        private static readonly HttpClient _http = new()
        {
            BaseAddress = new Uri("http://localhost:5102/"),
            Timeout = TimeSpan.FromSeconds(10)
        };


        public static async Task<List<string>> ListarGruposAsync(
            CancellationToken ct = default)
        {
            return await _http.GetFromJsonAsync<List<string>>(
                "api/dashboard/grupos", ct) ?? new();
        }


        public static async Task<(bool ok, string mensaje)> ProbarAsync()
        {
            try
            {
                var resp = await _http.GetAsync("api/emociones/estado");
                return (resp.IsSuccessStatusCode, await resp.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public static async Task<List<EmocionDto>> ListarEmocionesAsync() =>
            await _http.GetFromJsonAsync<List<EmocionDto>>("api/emociones") ?? new();

        public static async Task CrearEmocionAsync(EmocionDto dto)
        {
            var resp = await _http.PostAsJsonAsync("api/emociones", dto);
            resp.EnsureSuccessStatusCode();
        }

        

        // desde/hasta inclusivos. grupo = null -> todos los grupos.
        private static string Filtros(DateTime desde, DateTime hasta, string? grupo)
        {
            var inv = CultureInfo.InvariantCulture;
            string q = $"desde={desde.ToString("yyyy-MM-dd", inv)}" +
                       $"&hasta={hasta.ToString("yyyy-MM-dd", inv)}";

            if (!string.IsNullOrWhiteSpace(grupo))
                q += "&grupo=" + Uri.EscapeDataString(grupo);

            return q;
        }

        public static Task<ResumenDto?> ResumenAsync(
            DateTime desde, DateTime hasta, string? grupo, CancellationToken ct = default) =>
            _http.GetFromJsonAsync<ResumenDto>(
                $"api/dashboard/resumen?{Filtros(desde, hasta, grupo)}", ct);

        public static Task<List<SemanaDto>?> SemanasAsync(
            DateTime desde, DateTime hasta, string? grupo, CancellationToken ct = default) =>
            _http.GetFromJsonAsync<List<SemanaDto>>(
                $"api/dashboard/semanas?{Filtros(desde, hasta, grupo)}", ct);

        public static Task<List<NivelDto>?> NivelesAsync(
            DateTime desde, DateTime hasta, string? grupo, CancellationToken ct = default) =>
            _http.GetFromJsonAsync<List<NivelDto>>(
                $"api/dashboard/niveles?{Filtros(desde, hasta, grupo)}", ct);

        public static Task<List<AtencionDto>?> AtencionAsync(
            DateTime desde, DateTime hasta, string? grupo, CancellationToken ct = default) =>
            _http.GetFromJsonAsync<List<AtencionDto>>(
                $"api/dashboard/atencion?{Filtros(desde, hasta, grupo)}", ct);
    }

    public record EmocionDto(int Id, string Nombre);

    // ---- DTOs del dashboard ----

    public class ResumenDto
    {
        [JsonPropertyName("participantes_activos")] public double ParticipantesActivos { get; set; }
        [JsonPropertyName("sesiones")] public double Sesiones { get; set; }
        [JsonPropertyName("sesiones_completadas")] public double Completadas { get; set; }
        [JsonPropertyName("sesiones_abandonadas")] public double Abandonadas { get; set; }
        [JsonPropertyName("puntuacion_media")] public double? PuntuacionMedia { get; set; }
        [JsonPropertyName("resp_media_ms")] public double? RespMediaMs { get; set; }
        [JsonPropertyName("aciertos")] public double Aciertos { get; set; }
        [JsonPropertyName("errores")] public double Errores { get; set; }
    }

    public class SemanaDto
    {
        [JsonPropertyName("semana")] public int Semana { get; set; }
        [JsonPropertyName("aciertos")] public double Aciertos { get; set; }
        [JsonPropertyName("errores")] public double Errores { get; set; }
    }

    public class NivelDto
    {
        [JsonPropertyName("nivel")] public int Nivel { get; set; }
        [JsonPropertyName("aciertos")] public double Aciertos { get; set; }
        [JsonPropertyName("errores")] public double Errores { get; set; }
        [JsonPropertyName("registros")] public double Registros { get; set; }
        [JsonPropertyName("completados")] public double Completados { get; set; }
    }

    public class AtencionDto
    {
        [JsonPropertyName("codigo")] public string Codigo { get; set; } = "";
        [JsonPropertyName("motivo")] public string Motivo { get; set; } = "";
        [JsonPropertyName("estado")] public string Estado { get; set; } = "";
    }
}