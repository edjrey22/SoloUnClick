namespace SoloUnClick.Domain.Entities;

public class Opinion
{
    public int Id { get; set; }
    
    /// <summary>
    /// Título breve de la opinión
    /// </summary>
    public string Titulo { get; set; } = string.Empty;
    
    public string Comentario { get; set; } = string.Empty;
    public int Calificacion { get; set; } // 1-5
    public DateTime FechaOpinion { get; set; }

    // Relaciones
    public int PaqueteTuristicoId { get; set; }
    public PaqueteTuristico PaqueteTuristico { get; set; } = null!;

    public string ApplicationUserId { get; set; } = string.Empty;
    public ApplicationUser ApplicationUser { get; set; } = null!;
}
