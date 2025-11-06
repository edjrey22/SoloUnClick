# Sistema de Reservas - SoloUnClick

## ?? Implementación Completada

### ?? Funcionalidades Principales

#### 1. ReservasController (Protegido con [Authorize])
- **Crear Reserva** - GET y POST
- **Confirmación de Reserva** - GET
- **Mis Reservas** - GET (Lista de reservas del usuario)

---

## ??? Arquitectura

### ViewModels Creados

#### 1. ReservaCrearViewModel
```csharp
public class ReservaCrearViewModel
{
    public PaqueteTuristico Paquete { get; set; }
    public ReservaFormViewModel Formulario { get; set; }
}
```

#### 2. ReservaFormViewModel
```csharp
public class ReservaFormViewModel
{
    [Required]
    [DataType(DataType.Date)]
    public DateTime FechaViaje { get; set; }

    [Required]
    [Range(1, 20)]
    public int NumeroPasajeros { get; set; }

    public int PaqueteTuristicoId { get; set; }
    public decimal PrecioTotal { get; set; }
}
```

#### 3. ReservaConfirmacionViewModel
```csharp
public class ReservaConfirmacionViewModel
{
    public Reserva Reserva { get; set; }
    public PaqueteTuristico Paquete { get; set; }
}
```

---

## ?? Seguridad y Validaciones

### Controlador Protegido
```csharp
[Authorize]
public class ReservasController : Controller
```

### Validaciones Implementadas

#### En el Cliente (JavaScript):
- ? **Fecha mínima**: Mañana (no se puede reservar para hoy o el pasado)
- ? **Número de pasajeros**: Entre 1 y 20
- ? **Cálculo en tiempo real** del precio total
- ? **Botones +/- ** para incrementar/decrementar pasajeros
- ? **Validación de inputs** con HTML5

#### En el Servidor (C#):
- ? **ModelState válido**
- ? **Fecha de viaje futura** (debe ser posterior a hoy)
- ? **Rango de pasajeros** (1-20)
- ? **Paquete existe** antes de reservar
- ? **Usuario autenticado** requerido
- ? **Cálculo del precio total** en el servidor

---

## ?? Características del Formulario de Reserva

### 1. Información Mostrada

#### Resumen del Paquete (Sidebar):
- ? **Imagen principal** del paquete
- ? **Nombre y tipo de viaje**
- ? **Ubicación y región**
- ? **Duración** del viaje
- ? **Lista de inclusiones** (Alojamiento, Guía, Transporte, Seguro)

#### Formulario Principal:
- ? **Date picker** para fecha de viaje
  - HTML5 date input
  - Fecha mínima configurada (mañana)
  - Formato visual claro
  
- ? **Selector de pasajeros**
  - Input numérico central
  - Botones +/- para incrementar/decrementar
  - Rango de 1 a 20 pasajeros
  - Validación automática

- ? **Resumen de costos en tiempo real**
  - Precio por persona
  - Número de pasajeros
  - Total calculado dinámicamente

### 2. JavaScript Funcional

#### Cálculo Automático de Precio:
```javascript
function actualizarPrecioTotal() {
    const numeroPasajeros = parseInt(numeroPasajerosInput.value) || 1;
    const precioTotal = precioPorPersona * numeroPasajeros;
    
    // Actualizar displays
    displayPasajeros.textContent = numeroPasajeros;
    precioTotalDisplay.textContent = precioTotal.toFixed(2);
    precioTotalHidden.value = precioTotal.toFixed(2);
}
```

#### Características:
- ? **Event listeners** en input, change
- ? **Botones +/-** con validación de rango
- ? **Prevención de valores inválidos** en blur
- ? **Sincronización** entre input visible y campo hidden
- ? **Formato de moneda** con 2 decimales

---

## ?? Flujo de Reserva

### 1. Inicio de Reserva
```
Usuario hace click en "Reservar Ahora"
    ?
Verificación de autenticación
    ?
Si no está autenticado ? Redirige a Login
    ?
Si está autenticado ? Muestra formulario
```

### 2. Completar Formulario
```
Usuario selecciona fecha de viaje
    ?
Usuario elige número de pasajeros
    ?
JavaScript calcula precio total en tiempo real
    ?
Usuario revisa resumen
    ?
Usuario hace click en "Confirmar Reserva"
```

### 3. Procesamiento de Reserva
```
POST a ReservasController.Crear()
    ?
Validaciones en servidor
    ?
Obtener ApplicationUser autenticado
    ?
Crear entidad Reserva
    ?
BeginTransaction()
    ?
AddAsync() a UnitOfWork.Reservas
    ?
CommitTransactionAsync()
    ?
Redirigir a Confirmación con mensaje de éxito
```

---

## ?? Vistas Implementadas

### 1. Crear.cshtml
**Ruta**: `/Reservas/Crear/{paqueteId}`

**Características**:
- ? **Breadcrumb** de navegación
- ? **Layout de 2 columnas**: Formulario (7) + Resumen (5)
- ? **Sidebar sticky** con información del paquete
- ? **Formulario responsive** con Bootstrap 5
- ? **Validación visual** con mensajes de error
- ? **Botones de acción**: Confirmar / Cancelar
- ? **JavaScript inline** para cálculos

**Elementos del Formulario**:
```html
<!-- Date Picker -->
<input type="date" id="fechaViaje" min="mañana" />

<!-- Selector de Pasajeros -->
<button id="btnMenos">-</button>
<input type="number" id="numeroPasajeros" min="1" max="20" />
<button id="btnMas">+</button>

<!-- Resumen de Precio -->
<div>Precio por persona: S/ X.XX</div>
<div>Número de pasajeros: ×N</div>
<div>Total: S/ XXX.XX</div>
```

### 2. Confirmacion.cshtml
**Ruta**: `/Reservas/Confirmacion/{id}`

**Características**:
- ? **Animación de éxito** (check circle)
- ? **Número de reserva** destacado
- ? **Información completa** de la reserva:
  - Paquete con imagen
  - Fecha de viaje
  - Número de pasajeros
  - Fecha de reserva
  - Resumen de pago
- ? **Lista de próximos pasos**
- ? **Botones de acción**: Ver Mis Reservas / Volver al Inicio

**Seguridad**:
- ? Verifica que la reserva pertenece al usuario actual
- ? Redirige si no tiene permiso

### 3. MisReservas.cshtml
**Ruta**: `/Reservas/MisReservas`

**Características**:
- ? **Tabla responsive** con todas las reservas del usuario
- ? **Columnas**: Número, Paquete, Fecha, Pasajeros, Total, Estado, Acciones
- ? **Estados visuales**:
  - "Próximo" (verde) - Si fecha de viaje es futura
  - "Completado" (gris) - Si fecha de viaje pasó
- ? **Botón de acción** para ver detalles
- ? **Mensaje amigable** cuando no hay reservas
- ? **Ordenamiento** por fecha de reserva (más reciente primero)

---

## ?? Diseño y UX

### Estilos Implementados

#### Crear.cshtml:
```css
/* Sidebar sticky */
.sticky-top {
    position: sticky;
    top: 20px;
    z-index: 1020;
}

/* Mostrar spin buttons del input number */
input[type="number"]::-webkit-inner-spin-button {
    opacity: 1;
}

/* Efecto hover en botón submit */
#reservaForm button[type="submit"]:hover {
    transform: translateY(-2px);
    box-shadow: 0 0.5rem 1rem rgba(0,0,0,.15);
    transition: all 0.3s ease;
}
```

#### Confirmacion.cshtml:
```css
/* Animación del icono de éxito */
.success-icon {
    animation: scaleIn 0.5s ease-out;
}

@keyframes scaleIn {
    from { transform: scale(0); opacity: 0; }
    to { transform: scale(1); opacity: 1; }
}
```

#### MisReservas.cshtml:
```css
/* Alineación vertical en tabla */
.table td {
    vertical-align: middle;
}

/* Efecto hover en filas */
.table tbody tr:hover {
    background-color: #f8f9fa;
}
```

---

## ?? Métodos del Controlador

### 1. Crear (GET)
```csharp
[HttpGet]
public async Task<IActionResult> Crear(int paqueteId)
```

**Funcionalidad**:
- Obtiene el paquete por ID
- Crea ViewModel con valores por defecto
- Retorna vista con formulario

**Valores por Defecto**:
- Fecha de viaje: +7 días desde hoy
- Número de pasajeros: 1
- Precio total: PrecioPorPersona × 1

### 2. Crear (POST)
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Crear(ReservaFormViewModel model)
```

**Proceso**:
1. Validar ModelState
2. Validar fecha futura
3. Obtener paquete
4. Obtener usuario autenticado
5. Calcular precio total
6. Crear entidad Reserva
7. Usar transacción (BeginTransaction ? Add ? CommitTransaction)
8. Logging del éxito
9. Redirigir a Confirmación

**Manejo de Errores**:
- ModelState inválido ? Recargar vista con errores
- Fecha pasada ? Agregar error al ModelState
- Paquete no existe ? Mensaje de error + redirigir
- Usuario no autenticado ? Redirigir a Login
- Exception ? Rollback + Logging + Mensaje de error

### 3. Confirmacion (GET)
```csharp
[HttpGet]
public async Task<IActionResult> Confirmacion(int id)
```

**Seguridad**:
- Verifica que la reserva existe
- Verifica que pertenece al usuario actual
- Carga el paquete relacionado
- Muestra vista de confirmación

### 4. MisReservas (GET)
```csharp
[HttpGet]
public async Task<IActionResult> MisReservas()
```

**Funcionalidad**:
- Obtiene userId del usuario actual
- Busca todas las reservas del usuario
- Ordena por fecha de reserva descendente
- Retorna lista de reservas

---

## ?? Uso de IUnitOfWork

### Transacciones Implementadas

```csharp
// Iniciar transacción
await _unitOfWork.BeginTransactionAsync();

try
{
    // Operaciones de base de datos
    await _unitOfWork.Reservas.AddAsync(reserva);
    
    // Confirmar transacción
    await _unitOfWork.CommitTransactionAsync();
}
catch (Exception)
{
    // Revertir cambios en caso de error
    await _unitOfWork.RollbackTransactionAsync();
    throw;
}
```

### Repositorios Utilizados

```csharp
// Obtener paquete
var paquete = await _unitOfWork.PaquetesTuristicos.GetByIdAsync(paqueteId);

// Agregar reserva
await _unitOfWork.Reservas.AddAsync(reserva);

// Buscar reservas del usuario
var reservas = await _unitOfWork.Reservas.FindAsync(r => r.ApplicationUserId == userId);
```

---

## ?? Responsive Design

### Breakpoints Utilizados

#### Desktop (?992px):
- Layout 2 columnas: 7/5 (Formulario/Sidebar)
- Sidebar sticky activado
- Tabla completa visible

#### Tablet (768px - 991px):
- Layout apilado verticalmente
- Sidebar no sticky
- Tabla con scroll horizontal

#### Mobile (<768px):
- Todas las columnas al 100%
- Botones de acción en columna
- Tabla simplificada

---

## ?? Integración con el Sistema

### Rutas Agregadas

```
GET  /Reservas/Crear/{paqueteId}      ? Formulario de reserva
POST /Reservas/Crear                  ? Procesar reserva
GET  /Reservas/Confirmacion/{id}      ? Ver confirmación
GET  /Reservas/MisReservas            ? Lista de reservas
```

### Enlaces Actualizados

#### Details.cshtml (Paquetes):
```html
<a asp-controller="Reservas" asp-action="Crear" asp-route-paqueteId="@Model.Paquete.Id">
    Reservar Ahora
</a>
```

#### _Layout.cshtml:
```html
@if (User.Identity?.IsAuthenticated == true)
{
    <li class="nav-item">
        <a asp-controller="Reservas" asp-action="MisReservas">
            Mis Reservas
        </a>
    </li>
}
```

---

## ?? Datos Persistidos

### Entidad Reserva Creada

```csharp
var reserva = new Reserva
{
    FechaReserva = DateTime.Now,              // Fecha actual
    FechaViaje = model.FechaViaje,            // Del formulario
    NumeroPasajeros = model.NumeroPasajeros,  // Del formulario
    PrecioTotal = precioTotal,                // Calculado
    PaqueteTuristicoId = model.PaqueteTuristicoId,
    ApplicationUserId = userId                 // Del usuario autenticado
};
```

---

## ?? Logging Implementado

### Eventos Registrados

```csharp
// Éxito
_logger.LogInformation("Reserva creada exitosamente. ReservaId: {ReservaId}, UserId: {UserId}, PaqueteId: {PaqueteId}");

// Errores
_logger.LogError(ex, "Error al crear la reserva para el paquete {PaqueteId}", model.PaqueteTuristicoId);
_logger.LogError(ex, "Error al cargar el formulario de reserva para el paquete {PaqueteId}", paqueteId);
_logger.LogError(ex, "Error al cargar la confirmación de la reserva {ReservaId}", id);
_logger.LogError(ex, "Error al cargar las reservas del usuario");
```

---

## ? Checklist de Implementación

- [x] Crear ReservaViewModels (Crear, Form, Confirmación)
- [x] Crear ReservasController con [Authorize]
- [x] Implementar acción Crear (GET) con paqueteId
- [x] Implementar acción Crear (POST) con validaciones
- [x] Implementar acción Confirmacion (GET)
- [x] Implementar acción MisReservas (GET)
- [x] Crear vista Crear.cshtml con formulario
- [x] Implementar date picker con fecha mínima
- [x] Implementar selector de pasajeros con botones +/-
- [x] Implementar JavaScript para cálculo en tiempo real
- [x] Crear vista Confirmacion.cshtml
- [x] Crear vista MisReservas.cshtml
- [x] Actualizar Details.cshtml con botón de reserva
- [x] Actualizar _Layout.cshtml con enlace a Mis Reservas
- [x] Validaciones en cliente (HTML5 + JavaScript)
- [x] Validaciones en servidor (ModelState + Custom)
- [x] Uso de transacciones con UnitOfWork
- [x] Manejo de errores con try-catch
- [x] Logging de eventos importantes
- [x] Mensajes de feedback con TempData
- [x] Diseño responsive con Bootstrap 5
- [x] Estilos CSS personalizados
- [x] Animaciones y efectos visuales
- [x] Compilación exitosa

---

## ?? Resultado Final

El sistema de reservas está **completamente implementado y funcional**, incluyendo:

? **Formulario completo** de reserva con date picker y selector de pasajeros  
? **Cálculo automático** del precio total en JavaScript  
? **Validaciones robustas** en cliente y servidor  
? **Transacciones** usando IUnitOfWork.BeginTransaction/Commit  
? **Página de confirmación** con toda la información  
? **Lista de reservas** del usuario autenticado  
? **Protección con [Authorize]** en todo el controlador  
? **Integración completa** con el sistema existente  
? **Diseño responsive** y profesional  
? **Mensajes de feedback** claros para el usuario  
? **Logging completo** de operaciones  

¡El sistema está listo para procesar reservas de usuarios autenticados! ??
