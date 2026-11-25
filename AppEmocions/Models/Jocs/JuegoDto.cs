namespace AppEmocions.Models.Jocs;

public class JuegoDto
{
    public int JuegoId { get; set; }
    public string Nombre { get; set; } = "";
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
}
