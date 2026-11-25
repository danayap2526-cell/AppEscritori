namespace AppEmocions.Models.Dades;

public class SesionImportadaDto
{
    public string SesionId { get; set; } = "";
    public string CodigoInfant { get; set; } = "";
    public DateTime Data { get; set; }
    public string Joc { get; set; } = "";
    public bool Simulada { get; set; }
    public string Estat { get; set; } = "";
}
