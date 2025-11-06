using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SoloUnClick.Domain.Interfaces;
using SoloUnClick.Infrastructure.Data;
using SoloUnClick.Infrastructure.Repositories;

namespace SoloUnClick.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configurar DbContext
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        // Registrar Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Registrar repositorios genéricos (opcional, si se necesitan inyectar directamente)
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        return services;
    }
}
