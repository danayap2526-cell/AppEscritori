namespace AppEmocions.Models.Dashboard;

public class ComparativaNivelGrupoDto
{
    public string Grupo { get; set; } = "";
    public byte Nivel { get; set; }
    public double TasaAcierto { get; set; }
}
