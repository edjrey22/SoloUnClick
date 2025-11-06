using SoloUnClick.Domain.Entities;

namespace SoloUnClick.web.Models.ViewModels;

public class HomeIndexViewModel
{
    public string? BusquedaUbicacion { get; set; }
    public IEnumerable<PaqueteTuristico> DestinosPopulares { get; set; } = new List<PaqueteTuristico>();
    public IEnumerable<PaqueteTuristico> OfertasTemporada { get; set; } = new List<PaqueteTuristico>();
}
