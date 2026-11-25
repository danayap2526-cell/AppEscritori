namespace AppEmocions.Api.Data.Dades;

public sealed class SesionImportadaDto
{
    public string SesionId { get; set; } = string.Empty;
    public string CodigoInfant { get; set; } = string.Empty;
    public DateTime Data { get; set; }
    public string Joc { get; set; } = string.Empty;
    public bool Simulada { get; set; }
    public string Estat { get; set; } = "OK";
}

public sealed record IncidenciaCoherenciaDto(string Tipo, string Referencia, string Detalle);

public sealed record ResumenCalidadDatosDto(
    int TotalSesiones,
    int SesionesSinIncidencias,
    double PorcentajeCalidad,
    IEnumerable<IncidenciaCoherenciaDto> Incidencias
);