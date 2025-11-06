using SoloUnClick.Domain.Entities;
using SoloUnClick.Domain.Enums;
using X.PagedList;

namespace SoloUnClick.web.Models.ViewModels;

public class PaquetesBusquedaViewModel
{
    // Parámetros de búsqueda
    public string? Busqueda { get; set; }
    public TipoViaje? TipoViaje { get; set; }
    public decimal? PrecioMax { get; set; }
    public string? OrdenarPor { get; set; } // precio_asc, precio_desc, duracion_asc, duracion_desc, popularidad

    // Resultados paginados
    public IPagedList<PaqueteTuristico>? Paquetes { get; set; }

    // Para filtros en la vista
    public int PaginaActual { get; set; } = 1;
    public int TotalResultados { get; set; }
}
