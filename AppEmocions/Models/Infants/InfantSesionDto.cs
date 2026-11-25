namespace AppEmocions.Models.Infants;

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
