using System.Net;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SoloUnClick.Domain.Enums;
using SoloUnClick.Domain.Entities;
using SoloUnClick.Infrastructure.Data;

namespace SoloUnClick.Tests.Integration;

public class PagesAndControllersTests : IClassFixture<WebAppFactory>
{
    private readonly WebAppFactory _factory;

    public PagesAndControllersTests(WebAppFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Home_Index_Should_Load()
    {
        var client = _factory.CreateClient();
        var res = await client.GetAsync("/");
        res.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Can_Create_Paquete_And_Fetch_It_From_Controller_Route()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.PaquetesTuristicos.Add(new PaqueteTuristico
        {
            Nombre = "P1",
            DescripcionDetallada = "D",
            Ubicacion = "U",
            Region = "R",
            PrecioPorPersona = 100,
            DuracionDias = 2,
            TipoViaje = TipoViaje.Aventura,
            RutaImagenPrincipal = "img"
        });
        await db.SaveChangesAsync();

        var client = _factory.CreateClient();
        var res = await client.GetAsync("/Paquetes/Details/1");
        res.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
