using SoloUnClick.Domain.Enums;

namespace SoloUnClick.Domain.Entities;

public class Reserva
{
    public int Id { get; set; }
    public DateTime FechaReserva { get; set; }
    public DateTime FechaViaje { get; set; }
    public int NumeroPasajeros { get; set; }
    public decimal PrecioTotal { get; set; }
    
    /// <summary>
    /// Estado actual de la reserva
    /// </summary>
    public EstadoReserva Estado { get; set; } = EstadoReserva.Pendiente;

    // Relaciones
    public string ApplicationUserId { get; set; } = string.Empty;
    public ApplicationUser ApplicationUser { get; set; } = null!;

    public int PaqueteTuristicoId { get; set; }
    public PaqueteTuristico PaqueteTuristico { get; set; } = null!;
    
    /// <summary>
    /// Verifica si la reserva está confirmada y el viaje ya ocurrió
    /// </summary>
    public bool EstaCompletada() => Estado == EstadoReserva.Confirmada && FechaViaje < DateTime.Now;
}
