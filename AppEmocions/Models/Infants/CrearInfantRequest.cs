namespace AppEmocions.Models.Infants;

public record CrearInfantRequest(string Nombre, string Grupo, int? DocenteId, string? ContrasenaHash = null);
