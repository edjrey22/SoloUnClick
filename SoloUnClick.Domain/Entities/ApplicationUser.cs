using Microsoft.AspNetCore.Identity;

namespace SoloUnClick.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;

    /// <summary>
    /// Total acumulado de compras históricas del usuario
    /// </summary>
    public decimal TotalComprasHistoricas { get; set; } = 0m;

    /// <summary>
    /// Nivel de descuento del usuario basado en su lealtad (0.00 a 0.10 = 0% a 10%)
    /// </summary>
    public decimal NivelDescuento { get; set; } = 0m;

    // Navegación
    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    public ICollection<Opinion> Opiniones { get; set; } = new List<Opinion>();
}
