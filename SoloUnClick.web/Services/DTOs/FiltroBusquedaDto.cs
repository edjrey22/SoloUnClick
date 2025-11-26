namespace SoloUnClick.web.Services.DTOs;

public record FiltroBusquedaDto(
    RangoFechaDto RangoFechaInicio,
    decimal PresupuestoMaximo
);
