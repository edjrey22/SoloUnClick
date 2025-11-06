using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SoloUnClick.Domain.Entities;

namespace SoloUnClick.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // DbSets
    public DbSet<PaqueteTuristico> PaquetesTuristicos { get; set; }
    public DbSet<Opinion> Opiniones { get; set; }
    public DbSet<Reserva> Reservas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de PaqueteTuristico
        modelBuilder.Entity<PaqueteTuristico>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Nombre)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(p => p.DescripcionDetallada)
                .IsRequired()
                .HasMaxLength(2000);

            entity.Property(p => p.Ubicacion)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(p => p.Region)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(p => p.PrecioPorPersona)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(p => p.RutaImagenPrincipal)
                .HasMaxLength(500);

            entity.Property(p => p.TipoViaje)
                .IsRequired();
            
            entity.Property(p => p.PlazasDisponibles)
                .IsRequired()
                .HasDefaultValue(20);
            
            entity.Property(p => p.Activo)
                .IsRequired()
                .HasDefaultValue(true);
            
            entity.Property(p => p.Tags)
                .HasMaxLength(500)
                .HasDefaultValue(string.Empty);
            
            // Índice para búsquedas por Tags
            entity.HasIndex(p => p.Tags)
                .HasDatabaseName("IX_PaquetesTuristicos_Tags");

            // Relación uno-a-muchos con Reservas
            entity.HasMany(p => p.Reservas)
                .WithOne(r => r.PaqueteTuristico)
                .HasForeignKey(r => r.PaqueteTuristicoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación uno-a-muchos con Opiniones
            entity.HasMany(p => p.Opiniones)
                .WithOne(o => o.PaqueteTuristico)
                .HasForeignKey(o => o.PaqueteTuristicoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuración de Opinion
        modelBuilder.Entity<Opinion>(entity =>
        {
            entity.HasKey(o => o.Id);
            
            entity.Property(o => o.Titulo)
                .IsRequired()
                .HasMaxLength(100)
                .HasDefaultValue(string.Empty);

            entity.Property(o => o.Comentario)
                .IsRequired()
                .HasMaxLength(1000);

            entity.Property(o => o.Calificacion)
                .IsRequired();

            entity.Property(o => o.FechaOpinion)
                .IsRequired();

            // Relación con ApplicationUser
            entity.HasOne(o => o.ApplicationUser)
                .WithMany(u => u.Opiniones)
                .HasForeignKey(o => o.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Validación de rango para Calificacion (1-5)
            entity.ToTable(t => t.HasCheckConstraint("CK_Opinion_Calificacion", "[Calificacion] >= 1 AND [Calificacion] <= 5"));
        });

        // Configuración de Reserva
        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.HasKey(r => r.Id);

            entity.Property(r => r.FechaReserva)
                .IsRequired();

            entity.Property(r => r.FechaViaje)
                .IsRequired();

            entity.Property(r => r.NumeroPasajeros)
                .IsRequired();

            entity.Property(r => r.PrecioTotal)
                .HasPrecision(18, 2)
                .IsRequired();
            
            entity.Property(r => r.Estado)
                .IsRequired()
                .HasDefaultValue(SoloUnClick.Domain.Enums.EstadoReserva.Pendiente);
            
            // Índice para búsquedas por estado
            entity.HasIndex(r => r.Estado)
                .HasDatabaseName("IX_Reservas_Estado");

            // Relación con ApplicationUser
            entity.HasOne(r => r.ApplicationUser)
                .WithMany(u => u.Reservas)
                .HasForeignKey(r => r.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuración adicional de ApplicationUser
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(u => u.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.Apellido)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(u => u.TotalComprasHistoricas)
                .HasPrecision(18, 2)
                .HasDefaultValue(0m);
            
            entity.Property(u => u.NivelDescuento)
                .HasPrecision(5, 2)
                .HasDefaultValue(0m);
        });
    }
}
