namespace AppEmocions.Api.Data.Movil;

public sealed class SesionMovilDto
{
    public string Codigo { get; set; } = "";
    public string? Grupo { get; set; }
    public int Nivel { get; set; }
    public int DuracionS { get; set; }
    public decimal Puntuacion { get; set; }
    public int Intentos { get; set; }
    public int Errores { get; set; }
    public int Ayudas { get; set; }
    public int Aciertos { get; set; }
    public int? RespMediaMs { get; set; }
    public string? Dificultad { get; set; }
    public bool Completada { get; set; } = true;
    public bool Simulada { get; set; }
}

public sealed class AccesoMovilDto
{
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";
}

public sealed class SesionMovilHistorialDto
{
    public string SesionId { get; set; } = "";
    public DateTime IniciadaEn { get; set; }
    public int? DuracionS { get; set; }
    public decimal? Puntuacion { get; set; }
    public byte? NivelAlcanzado { get; set; }
    public int Errores { get; set; }
    public int Aciertos { get; set; }
}