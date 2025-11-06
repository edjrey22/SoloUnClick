using SoloUnClick.Domain.Entities;
using SoloUnClick.Domain.Enums;

namespace SoloUnClick.Domain.Interfaces;

/// <summary>
/// Repositorio especializado para PaqueteTuristico con métodos personalizados
/// </summary>
public interface IPaqueteTuristicoRepository : IGenericRepository<PaqueteTuristico>
{
    /// <summary>
    /// Obtiene los paquetes más populares basados en el número de reservas
    /// </summary>
    /// <param name="count">Número de paquetes a retornar</param>
    /// <returns>Lista de paquetes populares</returns>
    Task<IEnumerable<PaqueteTuristico>> GetPaquetesPopularesAsync(int count);
    
    /// <summary>
    /// Obtiene paquetes filtrados según criterios específicos
    /// </summary>
    Task<IEnumerable<PaqueteTuristico>> GetPaquetesFiltradosAsync(
        string? busqueda = null,
        TipoViaje? tipoViaje = null,
        decimal? precioMin = null,
        decimal? precioMax = null,
        int? duracionMin = null,
        int? duracionMax = null,
        bool soloActivos = true);
    
    /// <summary>
    /// Obtiene paquetes por tags
    /// </summary>
    Task<IEnumerable<PaqueteTuristico>> GetPaquetesPorTagsAsync(string tags);
    
    /// <summary>
    /// Obtiene paquetes con plazas disponibles
    /// </summary>
    Task<IEnumerable<PaqueteTuristico>> GetPaquetesConPlazasDisponiblesAsync(int minPlazas = 1);
    
    /// <summary>
    /// Obtiene un paquete con todas sus relaciones cargadas
    /// </summary>
    Task<PaqueteTuristico?> GetPaqueteCompletoAsync(int id);
}
