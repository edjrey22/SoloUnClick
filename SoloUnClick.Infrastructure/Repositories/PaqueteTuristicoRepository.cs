using Microsoft.EntityFrameworkCore;
using SoloUnClick.Domain.Entities;
using SoloUnClick.Domain.Enums;
using SoloUnClick.Domain.Interfaces;
using SoloUnClick.Infrastructure.Data;

namespace SoloUnClick.Infrastructure.Repositories;

public class PaqueteTuristicoRepository : GenericRepository<PaqueteTuristico>, IPaqueteTuristicoRepository
{
    public PaqueteTuristicoRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<PaqueteTuristico>> GetPaquetesPopularesAsync(int count)
    {
        return await _dbSet
            .Include(p => p.Reservas)
            .Include(p => p.Opiniones)
            .Where(p => p.Activo)
            .OrderByDescending(p => p.Reservas.Count)
            .ThenByDescending(p => p.Opiniones.Any() ? p.Opiniones.Average(o => o.Calificacion) : 0)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<PaqueteTuristico>> GetPaquetesFiltradosAsync(
        string? busqueda = null,
        TipoViaje? tipoViaje = null,
        decimal? precioMin = null,
        decimal? precioMax = null,
        int? duracionMin = null,
        int? duracionMax = null,
        bool soloActivos = true)
    {
        var query = _dbSet.AsQueryable();

        if (soloActivos)
        {
            query = query.Where(p => p.Activo);
        }

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var busquedaLower = busqueda.ToLower();
            query = query.Where(p =>
                p.Nombre.ToLower().Contains(busquedaLower) ||
                p.Ubicacion.ToLower().Contains(busquedaLower) ||
                p.Region.ToLower().Contains(busquedaLower) ||
                p.DescripcionDetallada.ToLower().Contains(busquedaLower) ||
                p.Tags.ToLower().Contains(busquedaLower));
        }

        if (tipoViaje.HasValue)
        {
            query = query.Where(p => p.TipoViaje == tipoViaje.Value);
        }

        if (precioMin.HasValue)
        {
            query = query.Where(p => p.PrecioPorPersona >= precioMin.Value);
        }

        if (precioMax.HasValue)
        {
            query = query.Where(p => p.PrecioPorPersona <= precioMax.Value);
        }

        if (duracionMin.HasValue)
        {
            query = query.Where(p => p.DuracionDias >= duracionMin.Value);
        }

        if (duracionMax.HasValue)
        {
            query = query.Where(p => p.DuracionDias <= duracionMax.Value);
        }

        return await query
            .Include(p => p.Opiniones)
            .OrderBy(p => p.Nombre)
            .ToListAsync();
    }

    public async Task<IEnumerable<PaqueteTuristico>> GetPaquetesPorTagsAsync(string tags)
    {
        if (string.IsNullOrWhiteSpace(tags))
            return new List<PaqueteTuristico>();

        var tagsArray = tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim().ToLower())
            .ToList();

        return await _dbSet
            .Where(p => p.Activo && tagsArray.Any(tag => p.Tags.ToLower().Contains(tag)))
            .Include(p => p.Opiniones)
            .ToListAsync();
    }

    public async Task<IEnumerable<PaqueteTuristico>> GetPaquetesConPlazasDisponiblesAsync(int minPlazas = 1)
    {
        return await _dbSet
            .Where(p => p.Activo && p.PlazasDisponibles >= minPlazas)
            .Include(p => p.Opiniones)
            .OrderBy(p => p.Nombre)
            .ToListAsync();
    }

    public async Task<PaqueteTuristico?> GetPaqueteCompletoAsync(int id)
    {
        return await _dbSet
            .Include(p => p.Reservas)
                .ThenInclude(r => r.ApplicationUser)
            .Include(p => p.Opiniones)
                .ThenInclude(o => o.ApplicationUser)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}
