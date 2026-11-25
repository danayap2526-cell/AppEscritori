namespace AppEmocions.Api.Dtos.Usuaris; public sealed record CrearDocenteRequest(string Usuario, string NombreVisible, string? Email, string Contrasena, string Rol, List<string> Grupos);
