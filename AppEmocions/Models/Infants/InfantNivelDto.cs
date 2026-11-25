namespace AppEmocions.Models.Infants;

public class InfantNivelDto
{
    public byte Nivel { get; set; }
    public int Registros { get; set; }
    public int Completados { get; set; }
    public int Aciertos { get; set; }
    public int Errores { get; set; }
    public double RespMediaMs { get; set; }
}
