namespace AppEmocions.Models.Dades;

public record ResumenCalidadDatosDto(
    int TotalSesiones,
    int SesionesSinIncidencias,
    double PorcentajeCalidad,
    List<IncidenciaCoherenciaDto> Incidencias
);
