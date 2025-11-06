using SoloUnClick.Domain.Entities;

namespace SoloUnClick.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    // Repositorios específicos
    IPaqueteTuristicoRepository PaquetesTuristicos { get; }
    IGenericRepository<Opinion> Opiniones { get; }
    IGenericRepository<Reserva> Reservas { get; }
    IGenericRepository<ApplicationUser> Usuarios { get; }

    // Métodos de transacción
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
