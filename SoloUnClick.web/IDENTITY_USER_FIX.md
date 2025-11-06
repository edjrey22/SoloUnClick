# ?? Solución: InvalidOperationException al Intentar Login

## ? Error Reportado

```
InvalidOperationException: No service for type 'Microsoft.AspNetCore.Identity.UserManager`1[Microsoft.AspNetCore.Identity.IdentityUser]' has been registered.
```

**Página afectada**: `/Identity/Account/Login`

---

## ?? Diagnóstico del Problema

### Causa Raíz

El problema ocurrió porque el archivo `_LoginPartial.cshtml` estaba intentando inyectar:
- `UserManager<IdentityUser>`
- `SignInManager<IdentityUser>`

Pero nuestra aplicación está configurada para usar:
- `UserManager<ApplicationUser>`
- `SignInManager<ApplicationUser>`

### Por Qué Ocurrió

Cuando se hace scaffolding de Identity con `AddDefaultUI()`, ASP.NET Core genera automáticamente un `_LoginPartial.cshtml` que usa `IdentityUser` por defecto. Este archivo no se actualiza automáticamente cuando usamos un modelo personalizado como `ApplicationUser`.

### Stack del Error

```
Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService
Microsoft.AspNetCore.Mvc.Razor.RazorPagePropertyActivator
Microsoft.AspNetCore.Mvc.Razor.RazorView.RenderPageCoreAsync
Microsoft.AspNetCore.Identity.UI.V5.Pages.Internal.Areas_Identity_Pages_V5__Layout.ExecuteAsync
```

El error ocurre cuando:
1. Usuario navega a `/Identity/Account/Login`
2. La página intenta cargar el layout `_Layout.cshtml`
3. El layout incluye `_LoginPartial.cshtml`
4. `_LoginPartial` intenta inyectar `UserManager<IdentityUser>`
5. El contenedor DI no encuentra este servicio ?
6. Excepción lanzada

---

## ? Solución Aplicada

### 1. Actualizar _LoginPartial.cshtml

**Ubicación**: `SoloUnClick.web\Views\Shared\_LoginPartial.cshtml`

**Antes (Incorrecto)**:
```csharp
@using Microsoft.AspNetCore.Identity

@inject SignInManager<IdentityUser> SignInManager
@inject UserManager<IdentityUser> UserManager
```

**Después (Correcto)**:
```csharp
@using Microsoft.AspNetCore.Identity
@using SoloUnClick.Domain.Entities

@inject SignInManager<ApplicationUser> SignInManager
@inject UserManager<ApplicationUser> UserManager
```

### 2. Actualizar _ViewImports.cshtml de Identity

**Ubicación**: `SoloUnClick.web\Areas\Identity\Pages\_ViewImports.cshtml`

**Agregado**:
```csharp
@using SoloUnClick.Domain.Entities
```

Esto asegura que todas las páginas de Identity tengan acceso a `ApplicationUser`.

---

## ?? Flujo Corregido

### Flujo de Login (Ahora)

```
1. Usuario navega a /Identity/Account/Login
   ?
2. Página Login.cshtml se carga
   ?
3. Layout _Layout.cshtml se renderiza
   ?
4. _LoginPartial.cshtml se incluye
   ?
5. Inyección de dependencias:
   - SignInManager<ApplicationUser> ?
   - UserManager<ApplicationUser> ?
   ?
6. Página se renderiza correctamente
   ?
7. Usuario ve formulario de login
```

### Servicios Registrados en DI Container

```csharp
// En Program.cs
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(...)
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();
```

**Servicios disponibles**:
- ? `UserManager<ApplicationUser>`
- ? `SignInManager<ApplicationUser>`
- ? `RoleManager<IdentityRole>`
- ? `UserStore<ApplicationUser>`
- ? `UserManager<IdentityUser>` (NO registrado)

---

## ?? Verificación

### Test 1: Navegar a Login

```
URL: https://localhost:7131/Identity/Account/Login
Resultado Esperado: ? Página carga correctamente
Resultado Antes: ? InvalidOperationException
Resultado Ahora: ? Página funciona
```

### Test 2: Navegar a Register

```
URL: https://localhost:7131/Identity/Account/Register
Resultado Esperado: ? Página carga correctamente
Resultado Antes: ? InvalidOperationException
Resultado Ahora: ? Página funciona
```

### Test 3: Ver Home cuando está autenticado

```
Escenario: Usuario autenticado navega a Home
_LoginPartial debe mostrar: "Hello [nombre]!" y botón Logout
Resultado Antes: ? Error al renderizar
Resultado Ahora: ? Muestra correctamente
```

---

## ?? Archivos Modificados

### 1. _LoginPartial.cshtml

**Cambios**:
- `IdentityUser` ? `ApplicationUser` en `@inject`
- Agregado `@using SoloUnClick.Domain.Entities`

**Impacto**:
- Navbar ahora muestra información del usuario correctamente
- No más errores de DI

### 2. _ViewImports.cshtml (Identity Area)

**Cambios**:
- Agregado `@using SoloUnClick.Domain.Entities`

**Impacto**:
- Todas las páginas de Identity pueden usar `ApplicationUser`
- Evita errores futuros

---

## ?? Prevención de Problemas Futuros

### 1. Al Hacer Scaffolding de Identity

Cuando generes nuevas páginas de Identity:

```bash
dotnet aspnet-codegenerator identity --files "Account.ConfirmEmail" --dbContext AppDbContext
```

**Después de generar**:
1. Abrir el archivo `.cshtml.cs` generado
2. Buscar todas las referencias a `IdentityUser`
3. Reemplazar por `ApplicationUser`
4. Agregar `using SoloUnClick.Domain.Entities;`

### 2. Verificar Archivos de Identity

Archivos que comúnmente tienen este problema:
- `_LoginPartial.cshtml`
- `Account/Manage/Index.cshtml.cs`
- `Account/Manage/_ManageNav.cshtml`
- Cualquier partial view que use `UserManager` o `SignInManager`

### 3. Usar Búsqueda Global

Para encontrar referencias a `IdentityUser`:

**En Visual Studio**:
```
Ctrl+Shift+F
Buscar: "IdentityUser"
Carpeta: Areas\Identity
```

**En VS Code / Command Line**:
```bash
grep -r "IdentityUser" SoloUnClick.web/Areas/Identity/
```

---

## ?? Comandos de Verificación

### Compilar

```bash
dotnet build
```

**Resultado esperado**: ? Build succeeded

### Ejecutar

```bash
dotnet run --project SoloUnClick.web
```

**Resultado esperado**: 
```
Now listening on: https://localhost:7131
Application started. Press Ctrl+C to shut down.
```

### Navegar

1. **Home**: `https://localhost:7131/` ?
2. **Login**: `https://localhost:7131/Identity/Account/Login` ?
3. **Register**: `https://localhost:7131/Identity/Account/Register` ?

---

## ?? Comparación Antes/Después

### Antes (Con Error)

```
Usuario ? /Identity/Account/Login
    ?
Layout carga _LoginPartial
    ?
@inject UserManager<IdentityUser>
    ?
DI Container busca UserManager<IdentityUser>
    ?
? No encontrado
    ?
InvalidOperationException
    ?
500 Internal Server Error
```

### Después (Corregido)

```
Usuario ? /Identity/Account/Login
    ?
Layout carga _LoginPartial
    ?
@inject UserManager<ApplicationUser>
    ?
DI Container busca UserManager<ApplicationUser>
    ?
? Encontrado
    ?
Servicio inyectado correctamente
    ?
Página renderiza
    ?
200 OK
```

---

## ?? Resultado Visual

### _LoginPartial Cuando NO Autenticado

```html
<ul class="navbar-nav">
    <li class="nav-item">
        <a href="/Identity/Account/Register">Register</a>
    </li>
    <li class="nav-item">
        <a href="/Identity/Account/Login">Login</a>
    </li>
</ul>
```

### _LoginPartial Cuando SÍ Autenticado

```html
<ul class="navbar-nav">
    <li class="nav-item">
        <a href="/Identity/Account/Manage/Index">Hello maria.garcia@example.com!</a>
    </li>
    <li class="nav-item">
        <form method="post" action="/Identity/Account/Logout">
            <button type="submit">Logout</button>
        </form>
    </li>
</ul>
```

---

## ? Checklist de Solución

- [x] Identificar archivo problemático (_LoginPartial.cshtml)
- [x] Cambiar `IdentityUser` por `ApplicationUser`
- [x] Agregar `using SoloUnClick.Domain.Entities`
- [x] Actualizar _ViewImports.cshtml de Identity
- [x] Compilar sin errores
- [x] Probar navegación a /Login
- [x] Probar navegación a /Register
- [x] Verificar que _LoginPartial renderiza correctamente
- [x] Confirmar que no hay más referencias a IdentityUser

---

## ?? Resultado Final

### Estado: ? RESUELTO

**Problema**:
- ? `InvalidOperationException` al intentar login
- ? No se podía acceder a páginas de Identity
- ? Error 500 en cualquier página que usara _LoginPartial

**Solución**:
- ? `_LoginPartial.cshtml` actualizado a `ApplicationUser`
- ? `_ViewImports.cshtml` con using correcto
- ? Compilación exitosa
- ? Login funciona
- ? Register funciona
- ? Navbar muestra información correctamente

### URLs Funcionando Ahora

- ? `https://localhost:7131/`
- ? `https://localhost:7131/Identity/Account/Login`
- ? `https://localhost:7131/Identity/Account/Register`
- ? Todas las rutas de la aplicación

### Credenciales de Prueba

```
maria.garcia@example.com / Pass123!
carlos.rodriguez@example.com / Pass123!
ana.martinez@example.com / Pass123!
luis.lopez@example.com / Pass123!
sofia.fernandez@example.com / Pass123!
```

---

## ?? Lección Aprendida

**Cuando uses Identity con un modelo personalizado (`ApplicationUser`)**:

1. ? Configura `AddIdentity<ApplicationUser, IdentityRole>`
2. ? Actualiza **TODOS** los archivos scaffolded
3. ? Busca referencias a `IdentityUser` en:
   - Páginas `.cshtml.cs`
   - Partial views `.cshtml`
   - _ViewImports
   - _LoginPartial
4. ? Reemplaza por `ApplicationUser`
5. ? Agrega `using` necesarios
6. ? Compila y prueba

**¡El login y registro ahora funcionan completamente!** ??

---

**Fecha de solución**: $(Get-Date)
**Estado**: ? OPERACIONAL
**Tiempo de inactividad**: ~15 minutos
**Impacto**: Sistema de autenticación completamente funcional
