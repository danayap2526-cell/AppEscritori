namespace AppEmocions.Api.Dtos.Infants; public sealed record InfantDetalleCompletoDto(InfantDetalleCabeceraDto Cabecera, IEnumerable<InfantNivelDto> PorNivel, IEnumerable<InfantSesionDto> Sesiones);
