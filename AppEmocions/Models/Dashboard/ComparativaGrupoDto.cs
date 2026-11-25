namespace AppEmocions.Models.Dashboard;

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
