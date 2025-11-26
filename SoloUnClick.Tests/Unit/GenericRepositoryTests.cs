using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SoloUnClick.Domain.Entities;
using SoloUnClick.Infrastructure.Data;
using SoloUnClick.Infrastructure.Repositories;
using Xunit;

namespace SoloUnClick.Tests.Unit;

public class GenericRepositoryTests
{
    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddAsync_Should_Add_Entity()
    {
        var ctx = CreateContext();
        var repo = new GenericRepository<PaqueteTuristico>(ctx);
        var entity = new PaqueteTuristico { Nombre = "Test", DescripcionDetallada = "D", Ubicacion = "U", Region = "R", PrecioPorPersona = 10, DuracionDias = 1, TipoViaje = SoloUnClick.Domain.Enums.TipoViaje.Aventura, RutaImagenPrincipal = "img" };

        await repo.AddAsync(entity);
        await ctx.SaveChangesAsync();

        (await repo.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task FindAsync_Should_Filter_By_Predicate()
    {
        var ctx = CreateContext();
        var repo = new GenericRepository<PaqueteTuristico>(ctx);
        await repo.AddAsync(new PaqueteTuristico { Nombre = "A", DescripcionDetallada = "D", Ubicacion = "U", Region = "R", PrecioPorPersona = 10, DuracionDias = 1, TipoViaje = SoloUnClick.Domain.Enums.TipoViaje.Aventura, RutaImagenPrincipal = "img" });
        await repo.AddAsync(new PaqueteTuristico { Nombre = "B", DescripcionDetallada = "D", Ubicacion = "U", Region = "R", PrecioPorPersona = 20, DuracionDias = 1, TipoViaje = SoloUnClick.Domain.Enums.TipoViaje.Aventura, RutaImagenPrincipal = "img" });
        await ctx.SaveChangesAsync();

        var result = await repo.FindAsync(p => p.PrecioPorPersona >= 15);
        result.Should().ContainSingle(r => r.Nombre == "B");
    }
}
