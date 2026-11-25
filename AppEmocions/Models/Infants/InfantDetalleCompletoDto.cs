namespace AppEmocions.Models.Infants;

public record InfantDetalleCompletoDto(
    InfantDetalleCabeceraDto Cabecera,
    List<InfantNivelDto> PorNivel,
    List<InfantSesionDto> Sesiones
);
