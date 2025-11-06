using SoloUnClick.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace SoloUnClick.web.Models.ViewModels;

public class PaqueteDetalleViewModel
{
    // Información del paquete
    public PaqueteTuristico Paquete { get; set; } = null!;
    
    // Opiniones del paquete
    public List<OpinionViewModel> Opiniones { get; set; } = new();
    
    // Estadísticas de opiniones
    public double CalificacionPromedio { get; set; }
    public int TotalOpiniones { get; set; }
    
    // Permiso para opinar
    public bool PuedeOpinar { get; set; }
    
    // Formulario para nueva opinión
    public NuevaOpinionViewModel NuevaOpinion { get; set; } = new();
}

public class OpinionViewModel
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Comentario { get; set; } = string.Empty;
    public int Calificacion { get; set; }
    public DateTime FechaOpinion { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string ApellidoUsuario { get; set; } = string.Empty;
}

public class NuevaOpinionViewModel
{
    [Required(ErrorMessage = "El título es obligatorio")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El título debe tener entre 3 y 100 caracteres")]
    [Display(Name = "Título")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El comentario es obligatorio")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "El comentario debe tener entre 10 y 1000 caracteres")]
    [Display(Name = "Comentario")]
    public string Comentario { get; set; } = string.Empty;

    [Required(ErrorMessage = "La calificación es obligatoria")]
    [Range(1, 5, ErrorMessage = "La calificación debe ser entre 1 y 5 estrellas")]
    [Display(Name = "Calificación")]
    public int Calificacion { get; set; }

    public int PaqueteTuristicoId { get; set; }
}
