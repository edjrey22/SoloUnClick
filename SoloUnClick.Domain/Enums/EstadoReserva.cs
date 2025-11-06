namespace SoloUnClick.Domain.Enums;

/// <summary>
/// Estados posibles de una reserva
/// </summary>
public enum EstadoReserva
{
    /// <summary>
    /// Reserva creada pero pendiente de confirmación
    /// </summary>
    Pendiente = 0,
    
    /// <summary>
    /// Reserva confirmada y pagada
    /// </summary>
    Confirmada = 1,
    
    /// <summary>
    /// Reserva cancelada por el usuario o el sistema
    /// </summary>
    Cancelada = 2
}
