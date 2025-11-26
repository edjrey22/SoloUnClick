using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SoloUnClick.Domain.Entities;
using SoloUnClick.Infrastructure.Data;
using SoloUnClick.Infrastructure.Repositories;
using SoloUnClick.web.Services;
using SoloUnClick.web.Services.DTOs;
using SoloUnClick.web.Services.Exceptions;
using Xunit;

namespace SoloUnClick.Tests.Unit;

public class PaqueteServiceTests
{
    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task BuscarOportunidadesAsync_Should_Throw_When_StartDate_After_EndDate()
    {
        var ctx = CreateContext();
        var repo = new GenericRepository<PaqueteTuristico>(ctx);
        var service = new PaqueteService(repo);

        var filtro = new FiltroBusquedaDto(
            new RangoFechaDto(
                new DateTime(2025, 12, 31),
                new DateTime(2025, 1, 1)
            ),
            1000m
        );

        var act = async () => await service.BuscarOportunidadesAsync(filtro);

        await act.Should().ThrowAsync<FiltroInvalidoException>()
            .WithMessage("La fecha de inicio no puede ser posterior a la fecha de fin.");
    }

    [Fact]
    public async Task BuscarOportunidadesAsync_Should_Return_Active_Packages_With_Available_Slots()
    {
        var ctx = CreateContext();
        var repo = new GenericRepository<PaqueteTuristico>(ctx);
        var service = new PaqueteService(repo);

        var fechaPaquete = new DateTime(2025, 6, 15);

        // Paquete activo con plazas
        await repo.AddAsync(new PaqueteTuristico
        {
            Nombre = "Viaje Activo",
            DescripcionDetallada = "D",
            Ubicacion = "U",
            Region = "R",
            PrecioPorPersona = 100,
            DuracionDias = 5,
            TipoViaje = SoloUnClick.Domain.Enums.TipoViaje.Aventura,
            RutaImagenPrincipal = "img",
            Activo = true,
            PlazasDisponibles = 10,
            FechaInicio = fechaPaquete
        });

        // Paquete inactivo (no debe retornar)
        await repo.AddAsync(new PaqueteTuristico
        {
            Nombre = "Viaje Inactivo",
            DescripcionDetallada = "D",
            Ubicacion = "U",
            Region = "R",
            PrecioPorPersona = 100,
            DuracionDias = 5,
            TipoViaje = SoloUnClick.Domain.Enums.TipoViaje.Aventura,
            RutaImagenPrincipal = "img",
            Activo = false,
            PlazasDisponibles = 10,
            FechaInicio = fechaPaquete
        });

        // Paquete sin plazas (no debe retornar)
        await repo.AddAsync(new PaqueteTuristico
        {
            Nombre = "Sin Plazas",
            DescripcionDetallada = "D",
            Ubicacion = "U",
            Region = "R",
            PrecioPorPersona = 100,
            DuracionDias = 5,
            TipoViaje = SoloUnClick.Domain.Enums.TipoViaje.Aventura,
            RutaImagenPrincipal = "img",
            Activo = true,
            PlazasDisponibles = 0,
            FechaInicio = fechaPaquete
        });

        await ctx.SaveChangesAsync();

        var filtro = new FiltroBusquedaDto(
            new RangoFechaDto(
                new DateTime(2025, 1, 1),
                new DateTime(2025, 12, 31)
            ),
            1000m
        );

        var result = await service.BuscarOportunidadesAsync(filtro);

        result.Should().ContainSingle();
        result[0].Nombre.Should().Be("Viaje Activo");
    }

    [Fact]
    public async Task BuscarOportunidadesAsync_Should_Filter_By_Date_Range()
    {
        var ctx = CreateContext();
        var repo = new GenericRepository<PaqueteTuristico>(ctx);
        var service = new PaqueteService(repo);

        // Paquete dentro del rango
        await repo.AddAsync(new PaqueteTuristico
        {
            Nombre = "Dentro del Rango",
            DescripcionDetallada = "D",
            Ubicacion = "U",
            Region = "R",
            PrecioPorPersona = 100,
            DuracionDias = 5,
            TipoViaje = SoloUnClick.Domain.Enums.TipoViaje.Aventura,
            RutaImagenPrincipal = "img",
            Activo = true,
            PlazasDisponibles = 10,
            FechaInicio = new DateTime(2025, 6, 15)
        });

        // Paquete fuera del rango
        await repo.AddAsync(new PaqueteTuristico
        {
            Nombre = "Fuera del Rango",
            DescripcionDetallada = "D",
            Ubicacion = "U",
            Region = "R",
            PrecioPorPersona = 100,
            DuracionDias = 5,
            TipoViaje = SoloUnClick.Domain.Enums.TipoViaje.Aventura,
            RutaImagenPrincipal = "img",
            Activo = true,
            PlazasDisponibles = 10,
            FechaInicio = new DateTime(2025, 12, 15)
        });

        await ctx.SaveChangesAsync();

        var filtro = new FiltroBusquedaDto(
            new RangoFechaDto(
                new DateTime(2025, 6, 1),
                new DateTime(2025, 6, 30)
            ),
            1000m
        );

        var result = await service.BuscarOportunidadesAsync(filtro);

        result.Should().ContainSingle();
        result[0].Nombre.Should().Be("Dentro del Rango");
    }

    [Fact]
    public async Task BuscarOportunidadesAsync_Should_Filter_By_MaxBudget()
    {
        var ctx = CreateContext();
        var repo = new GenericRepository<PaqueteTuristico>(ctx);
        var service = new PaqueteService(repo);

        var fechaPaquete = new DateTime(2025, 6, 15);

        // Paquete dentro del presupuesto
        await repo.AddAsync(new PaqueteTuristico
        {
            Nombre = "Económico",
            DescripcionDetallada = "D",
            Ubicacion = "U",
            Region = "R",
            PrecioPorPersona = 50,
            DuracionDias = 5,
            TipoViaje = SoloUnClick.Domain.Enums.TipoViaje.Aventura,
            RutaImagenPrincipal = "img",
            Activo = true,
            PlazasDisponibles = 10,
            FechaInicio = fechaPaquete
        });

        // Paquete fuera del presupuesto
        await repo.AddAsync(new PaqueteTuristico
        {
            Nombre = "Costoso",
            DescripcionDetallada = "D",
            Ubicacion = "U",
            Region = "R",
            PrecioPorPersona = 200,
            DuracionDias = 5,
            TipoViaje = SoloUnClick.Domain.Enums.TipoViaje.Aventura,
            RutaImagenPrincipal = "img",
            Activo = true,
            PlazasDisponibles = 10,
            FechaInicio = fechaPaquete
        });

        await ctx.SaveChangesAsync();

        var filtro = new FiltroBusquedaDto(
            new RangoFechaDto(
                new DateTime(2025, 1, 1),
                new DateTime(2025, 12, 31)
            ),
            100m
        );

        var result = await service.BuscarOportunidadesAsync(filtro);

        result.Should().ContainSingle();
        result[0].Nombre.Should().Be("Económico");
    }

    [Fact]
    public async Task BuscarOportunidadesAsync_Should_Mark_EsUltimaHora_When_Less_Than_3_Slots()
    {
        var ctx = CreateContext();
        var repo = new GenericRepository<PaqueteTuristico>(ctx);
        var service = new PaqueteService(repo);

        var fechaPaquete = new DateTime(2025, 6, 15);

        // Paquete con 2 plazas (ultima hora)
        await repo.AddAsync(new PaqueteTuristico
        {
            Nombre = "Ultima Hora",
            DescripcionDetallada = "D",
            Ubicacion = "U",
            Region = "R",
            PrecioPorPersona = 100,
            DuracionDias = 5,
            TipoViaje = SoloUnClick.Domain.Enums.TipoViaje.Aventura,
            RutaImagenPrincipal = "img",
            Activo = true,
            PlazasDisponibles = 2,
            FechaInicio = fechaPaquete
        });

        // Paquete con 5 plazas (no ultima hora)
        await repo.AddAsync(new PaqueteTuristico
        {
            Nombre = "Normal",
            DescripcionDetallada = "D",
            Ubicacion = "U",
            Region = "R",
            PrecioPorPersona = 100,
            DuracionDias = 5,
            TipoViaje = SoloUnClick.Domain.Enums.TipoViaje.Aventura,
            RutaImagenPrincipal = "img",
            Activo = true,
            PlazasDisponibles = 5,
            FechaInicio = fechaPaquete
        });

        await ctx.SaveChangesAsync();

        var filtro = new FiltroBusquedaDto(
            new RangoFechaDto(
                new DateTime(2025, 1, 1),
                new DateTime(2025, 12, 31)
            ),
            1000m
        );

        var result = await service.BuscarOportunidadesAsync(filtro);

        result.Should().HaveCount(2);
        result.First(r => r.Nombre == "Ultima Hora").EsUltimaHora.Should().BeTrue();
        result.First(r => r.Nombre == "Normal").EsUltimaHora.Should().BeFalse();
    }
}
