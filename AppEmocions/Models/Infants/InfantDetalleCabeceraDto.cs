namespace AppEmocions.Models.Infants;

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
