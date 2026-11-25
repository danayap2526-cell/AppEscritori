namespace AppEmocions.Api.Data.Usuaris;

public sealed class DocenteDto
{
    public int DocenteId { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public string NombreVisible { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Rol { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public DateTime? UltimoAcceso { get; set; }
    public List<string> Grupos { get; set; } = new();
}

public sealed record CrearDocenteRequest(
    string Usuario,
    string NombreVisible,
    string? Email,
    string Contrasena,
    string Rol,
    List<string> Grupos
);

public sealed record CambiarContrasenaRequest(string ContrasenaActual, string ContrasenaNueva);