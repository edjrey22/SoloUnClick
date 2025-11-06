using SoloUnClick.Domain.Enums;

namespace SoloUnClick.Domain.Entities;

public class PaqueteTuristico
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string DescripcionDetallada { get; set; } = string.Empty;
    public string Ubicacion { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public decimal PrecioPorPersona { get; set; }
    public int DuracionDias { get; set; }
    public TipoViaje TipoViaje { get; set; }
    public string RutaImagenPrincipal { get; set; } = string.Empty;
    
    /// <summary>
    /// Número de plazas disponibles para este paquete
    /// </summary>
    public int PlazasDisponibles { get; set; } = 20;
    
    /// <summary>
    /// Indica si el paquete está activo y disponible para reserva
    /// </summary>
    public bool Activo { get; set; } = true;
    
    /// <summary>
    /// Tags separados por comas para facilitar búsquedas (ej: "aventura,montaña,trekking")
    /// </summary>
    public string Tags { get; set; } = string.Empty;

    // Navegación
    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    public ICollection<Opinion> Opiniones { get; set; } = new List<Opinion>();
    
    /// <summary>
    /// Calcula la calificación promedio basada en las opiniones
    /// </summary>
    public double ObtenerCalificacionPromedio()
    {
        if (Opiniones == null || !Opiniones.Any())
            return 0;
            
        return Opiniones.Average(o => o.Calificacion);
    }
}
