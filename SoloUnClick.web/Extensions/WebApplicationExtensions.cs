using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SoloUnClick.Domain.Entities;
using SoloUnClick.Infrastructure.Data;
using SoloUnClick.Infrastructure.Seed;

namespace SoloUnClick.web.Extensions;

public static class WebApplicationExtensions
{
    /// <summary>
    /// Aplica migraciones pendientes y ejecuta el seed de datos
    /// </summary>
    public static async Task<WebApplication> UseDatabaseMigrationAndSeedAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<WebApplication>>();

        try
        {
            // Obtener el contexto de base de datos
            var context = services.GetRequiredService<AppDbContext>();
            
            logger.LogInformation("Verificando estado de la base de datos...");

            // Verificar si la base de datos puede conectarse
            var canConnect = await context.Database.CanConnectAsync();
            
            if (!canConnect)
            {
                logger.LogWarning("No se puede conectar a la base de datos. Verificando migraciones...");
            }

            // Aplicar migraciones pendientes automáticamente
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            
            if (pendingMigrations.Any())
            {
                logger.LogInformation("Aplicando {Count} migraciones pendientes...", pendingMigrations.Count());
                foreach (var migration in pendingMigrations)
                {
                    logger.LogInformation("  - {Migration}", migration);
                }
                
                await context.Database.MigrateAsync();
                logger.LogInformation("Migraciones aplicadas exitosamente");
            }
            else
            {
                logger.LogInformation("No hay migraciones pendientes. Base de datos actualizada.");
            }

            // Ejecutar el seeder
            logger.LogInformation("Verificando datos de seed...");
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var seederLogger = services.GetRequiredService<ILogger<DataSeeder>>();
            
            var seeder = new DataSeeder(context, userManager, seederLogger);
            await seeder.SeedAsync();
            
            logger.LogInformation("Proceso de inicialización de base de datos completado exitosamente");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error durante la inicialización de la base de datos");
            
            // En desarrollo, podemos querer que la aplicación falle
            // En producción, podríamos querer continuar
            if (app.Environment.IsDevelopment())
            {
                throw;
            }
        }

        return app;
    }
}
