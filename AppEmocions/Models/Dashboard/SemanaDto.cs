namespace AppEmocions.Models.Dashboard;

public class SemanaDto
{
    public int Semana { get; set; }
    public string? Grupo { get; set; }
    public double Aciertos { get; set; }
    public double Errores { get; set; }
    public double TasaAcierto { get; set; }
}
