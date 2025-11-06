using Microsoft.EntityFrameworkCore.Storage;
using SoloUnClick.Domain.Entities;
using SoloUnClick.Domain.Interfaces;
using SoloUnClick.Infrastructure.Data;

namespace SoloUnClick.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;

    // Repositorios lazy-loaded
    private IPaqueteTuristicoRepository? _paquetesTuristicos;
    private IGenericRepository<Opinion>? _opiniones;
    private IGenericRepository<Reserva>? _reservas;
    private IGenericRepository<ApplicationUser>? _usuarios;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    // Propiedades que inicializan los repositorios bajo demanda
    public IPaqueteTuristicoRepository PaquetesTuristicos
    {
        get
        {
            _paquetesTuristicos ??= new PaqueteTuristicoRepository(_context);
            return _paquetesTuristicos;
        }
    }

    public IGenericRepository<Opinion> Opiniones
    {
        get
        {
            _opiniones ??= new GenericRepository<Opinion>(_context);
            return _opiniones;
        }
    }

    public IGenericRepository<Reserva> Reservas
    {
        get
        {
            _reservas ??= new GenericRepository<Reserva>(_context);
            return _reservas;
        }
    }

    public IGenericRepository<ApplicationUser> Usuarios
    {
        get
        {
            _usuarios ??= new GenericRepository<ApplicationUser>(_context);
            return _usuarios;
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
            }
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
