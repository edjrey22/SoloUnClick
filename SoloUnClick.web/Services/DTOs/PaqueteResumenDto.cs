namespace SoloUnClick.web.Services.DTOs;

public record PaqueteResumenDto
{
    public int Id { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public decimal PrecioBase { get; init; }
    public DateTime FechaInicio { get; init; }
    public int PlazasDisponibles { get; init; }
    public bool EsUltimaHora { get; init; }
}
