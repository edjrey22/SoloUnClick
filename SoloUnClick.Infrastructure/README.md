# Patrón Repository y Unit of Work - SoloUnClick

## ?? Estructura Implementada

### Domain Layer (SoloUnClick.Domain)

#### Interfaces
- **`IGenericRepository<T>`** - Define operaciones CRUD genéricas
- **`IUnitOfWork`** - Coordina repositorios y transacciones

### Infrastructure Layer (SoloUnClick.Infrastructure)

#### Implementaciones
- **`GenericRepository<T>`** - Implementación concreta del repositorio genérico
- **`UnitOfWork`** - Implementación del patrón Unit of Work con lazy loading de repositorios

#### Extensiones
- **`ServiceCollectionExtensions`** - Métodos de extensión para configurar servicios en DI

---

## ?? Configuración en Program.cs (Razor Pages)

### 1. Agregar el Connection String en `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SoloUnClickDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### 2. Registrar servicios en `Program.cs`

```csharp
using SoloUnClick.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios de Razor Pages
builder.Services.AddRazorPages();

// Agregar servicios de Infrastructure (DbContext + UnitOfWork + Repositorios)
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

// Configurar el pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
```

---

## ?? Uso en Razor Pages

### Ejemplo: PageModel usando IUnitOfWork

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SoloUnClick.Domain.Interfaces;
using SoloUnClick.Domain.Entities;

namespace SoloUnClick.web.Pages.Paquetes
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public IndexModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<PaqueteTuristico> Paquetes { get; set; } = new List<PaqueteTuristico>();

        public async Task OnGetAsync()
        {
            // Obtener todos los paquetes turísticos
            Paquetes = await _unitOfWork.PaquetesTuristicos.GetAllAsync();
        }
    }
}
```

### Ejemplo: Crear una Reserva

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SoloUnClick.Domain.Interfaces;
using SoloUnClick.Domain.Entities;

namespace SoloUnClick.web.Pages.Reservas
{
    public class CreateModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public Reserva Reserva { get; set; } = new();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Agregar la reserva
            await _unitOfWork.Reservas.AddAsync(Reserva);
            
            // Guardar cambios
            await _unitOfWork.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
```

### Ejemplo: Uso con Transacciones

```csharp
public async Task<IActionResult> OnPostReservarConOpinionAsync()
{
    try
    {
        // Iniciar transacción
        await _unitOfWork.BeginTransactionAsync();

        // Crear reserva
        var reserva = new Reserva
        {
            FechaReserva = DateTime.Now,
            FechaViaje = DateTime.Now.AddDays(30),
            NumeroPasajeros = 2,
            PrecioTotal = 1500.00m,
            PaqueteTuristicoId = 1,
            ApplicationUserId = "user-id"
        };
        await _unitOfWork.Reservas.AddAsync(reserva);

        // Crear opinión
        var opinion = new Opinion
        {
            Comentario = "¡Excelente paquete!",
            Calificacion = 5,
            FechaOpinion = DateTime.Now,
            PaqueteTuristicoId = 1,
            ApplicationUserId = "user-id"
        };
        await _unitOfWork.Opiniones.AddAsync(opinion);

        // Confirmar transacción
        await _unitOfWork.CommitTransactionAsync();

        return RedirectToPage("./Success");
    }
    catch (Exception)
    {
        await _unitOfWork.RollbackTransactionAsync();
        throw;
    }
}
```

---

## ?? Métodos Disponibles en IGenericRepository<T>

### Consulta
- `GetByIdAsync(int id)` - Obtener entidad por ID
- `GetAllAsync()` - Obtener todas las entidades
- `FindAsync(Expression<Func<T, bool>> predicate)` - Buscar con filtro
- `FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)` - Primer elemento o null

### Escritura
- `AddAsync(T entity)` - Agregar entidad
- `AddRangeAsync(IEnumerable<T> entities)` - Agregar múltiples entidades
- `Update(T entity)` - Actualizar entidad
- `Remove(T entity)` - Eliminar entidad
- `RemoveRange(IEnumerable<T> entities)` - Eliminar múltiples entidades

### Auxiliares
- `ExistsAsync(Expression<Func<T, bool>> predicate)` - Verificar existencia
- `CountAsync(Expression<Func<T, bool>>? predicate)` - Contar elementos

---

## ?? Próximos Pasos

1. **Crear las migraciones de Entity Framework Core:**
   ```bash
   dotnet ef migrations add InitialCreate --project SoloUnClick.Infrastructure --startup-project SoloUnClick.web
   dotnet ef database update --project SoloUnClick.Infrastructure --startup-project SoloUnClick.web
   ```

2. **Configurar Identity** (si es necesario para autenticación)

3. **Crear repositorios específicos** si necesitas lógica personalizada más allá del CRUD genérico

---

## ?? Beneficios del Patrón Implementado

? **Separación de responsabilidades** - Domain, Infrastructure y Web bien separados  
? **Testeable** - Fácil de mockear las interfaces para pruebas unitarias  
? **Reutilizable** - Operaciones CRUD comunes centralizadas  
? **Transaccional** - Soporte para transacciones con commit/rollback  
? **Lazy Loading** - Repositorios se instancian solo cuando se necesitan  
? **Inyección de Dependencias** - Todo configurado con DI de .NET 9
