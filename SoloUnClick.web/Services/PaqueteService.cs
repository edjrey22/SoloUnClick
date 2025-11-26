using SoloUnClick.Domain.Entities;
using SoloUnClick.Domain.Interfaces;
using SoloUnClick.web.Services.DTOs;
using SoloUnClick.web.Services.Exceptions;

namespace SoloUnClick.web.Services;

public class PaqueteService
{
    private readonly IGenericRepository<PaqueteTuristico> _paqueteRepository;

    public PaqueteService(IGenericRepository<PaqueteTuristico> paqueteRepository)
    {
        _paqueteRepository = paqueteRepository;
    }

    public async Task<List<PaqueteResumenDto>> BuscarOportunidadesAsync(FiltroBusquedaDto filtro)
    {
        // Validación del filtro
        if (filtro.RangoFechaInicio.Inicio > filtro.RangoFechaInicio.Fin)
        {
            throw new FiltroInvalidoException("La fecha de inicio no puede ser posterior a la fecha de fin.");
        }

        // Consulta con filtros aplicados
        var paquetes = await _paqueteRepository.FindAsync(p =>
            p.Activo &&
            p.PlazasDisponibles > 0 &&
            p.FechaInicio >= filtro.RangoFechaInicio.Inicio &&
            p.FechaInicio <= filtro.RangoFechaInicio.Fin &&
            p.PrecioPorPersona <= filtro.PresupuestoMaximo
        );

        // Proyección a DTOs
        var resultado = paquetes.Select(p => new PaqueteResumenDto
        {
            Id = p.Id,
            Nombre = p.Nombre,
            PrecioBase = p.PrecioPorPersona,
            FechaInicio = p.FechaInicio,
            PlazasDisponibles = p.PlazasDisponibles,
            EsUltimaHora = p.PlazasDisponibles < 3
        }).ToList();

        return resultado;
    }
}
