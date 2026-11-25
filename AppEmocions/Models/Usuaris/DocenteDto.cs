namespace AppEmocions.Models.Usuaris;

public class DocenteDto
{
    public int DocenteId { get; set; }
    public string Usuario { get; set; } = "";
    public string NombreVisible { get; set; } = "";
    public string? Email { get; set; }
    public string Rol { get; set; } = "";
    public bool Activo { get; set; }
    public DateTime? UltimoAcceso { get; set; }
    public List<string> Grupos { get; set; } = new();
}
