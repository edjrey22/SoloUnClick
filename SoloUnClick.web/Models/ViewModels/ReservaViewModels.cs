using SoloUnClick.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace SoloUnClick.web.Models.ViewModels;

public class ReservaCrearViewModel
{
    // Información del paquete
    public PaqueteTuristico Paquete { get; set; } = null!;
    
    // Formulario de reserva
    public ReservaFormViewModel Formulario { get; set; } = new();
}

public class ReservaFormViewModel
{
    [Required(ErrorMessage = "La fecha de viaje es obligatoria")]
    [Display(Name = "Fecha de Viaje")]
    [DataType(DataType.Date)]
    public DateTime FechaViaje { get; set; } = DateTime.Now.AddDays(7);

    [Required(ErrorMessage = "El número de pasajeros es obligatorio")]
    [Range(1, 20, ErrorMessage = "El número de pasajeros debe estar entre 1 y 20")]
    [Display(Name = "Número de Pasajeros")]
    public int NumeroPasajeros { get; set; } = 1;

    public int PaqueteTuristicoId { get; set; }
    
    [Display(Name = "Precio Total")]
    public decimal PrecioTotal { get; set; }
}

public class ReservaConfirmacionViewModel
{
    public Reserva Reserva { get; set; } = null!;
    public PaqueteTuristico Paquete { get; set; } = null!;
}
