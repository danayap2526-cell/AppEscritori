using System.Net.Http.Json;

namespace AppEmocions.Data
{
    public static class ApiClient
    {
        private static readonly HttpClient _http = new()
        {
            BaseAddress = new Uri("http://localhost:5102/"),
            Timeout = TimeSpan.FromSeconds(10)
        };

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
    }

    public record EmocionDto(int Id, string Nombre);
}