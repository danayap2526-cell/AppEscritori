namespace AppEmocions.Models.Usuaris;

public record CrearDocenteRequest(
    string Usuario,
    string NombreVisible,
    string? Email,
    string Contrasena,
    string Rol,
    List<string> Grupos
);
