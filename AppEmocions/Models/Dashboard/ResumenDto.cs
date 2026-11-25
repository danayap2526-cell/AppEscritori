using System.Text.Json.Serialization;

namespace AppEmocions.Models.Dashboard;

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
