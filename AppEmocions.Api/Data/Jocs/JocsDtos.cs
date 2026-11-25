namespace AppEmocions.Api.Data.Jocs;

public sealed class JuegoDto
{
    public int JuegoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
}

public sealed record GuardarJuegoRequest(string Nombre, string? Descripcion, bool Activo);