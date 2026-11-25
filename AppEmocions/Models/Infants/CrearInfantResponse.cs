using System.Text.Json.Serialization;

namespace AppEmocions.Models.Infants;

public class CrearInfantResponse
{
    [JsonPropertyName("participante_id")] public int ParticipanteId { get; set; }
    public string Codigo { get; set; } = "";
}
