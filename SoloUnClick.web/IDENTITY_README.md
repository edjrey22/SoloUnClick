# Integración de ASP.NET Core Identity - SoloUnClick

## ?? Implementación Completada

### ?? Objetivo

Integrar completamente ASP.NET Core Identity en SoloUnClick.web con páginas personalizadas de registro que incluyen campos adicionales (Nombre y Apellido) del modelo ApplicationUser.

---

## ??? Arquitectura de Identity

### Configuración en Program.cs

```csharp
// Configurar Identity con ApplicationUser
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Configuración de contraseñas
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    
    // Configuración de usuario
    options.User.RequireUniqueEmail = true;
    
    // Configuración de login - NO requerir confirmación de email para desarrollo
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI(); // Agregar soporte para Razor Pages de Identity

// Configurar cookies de autenticación
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// Agregar Razor Pages (necesario para Identity UI)
builder.Services.AddRazorPages();
```

**Características**:
- ? Usa `ApplicationUser` en lugar de `IdentityUser`
- ? Contraseñas requieren mayúsculas, minúsculas y dígitos
- ? Emails únicos requeridos
- ? NO requiere confirmación de email (desarrollo)
- ? Rutas personalizadas configuradas

---

## ?? Paquetes NuGet Instalados

| Paquete | Versión | Propósito |
|---------|---------|-----------|
| Microsoft.AspNetCore.Identity.UI | 9.0.10 | UI predefinida de Identity |
| Microsoft.VisualStudio.Web.CodeGeneration.Design | 9.0.0 | Herramienta de scaffolding |
| Microsoft.EntityFrameworkCore.Tools | 9.0.10 | Herramientas de EF Core |

---

## ?? Páginas Personalizadas Creadas

### Estructura de Archivos

```
SoloUnClick.web/
??? Areas/
    ??? Identity/
        ??? Pages/
            ??? _ViewImports.cshtml
            ??? Account/
                ??? Register.cshtml
                ??? Register.cshtml.cs
                ??? Login.cshtml
                ??? Login.cshtml.cs
                ??? Logout.cshtml
                ??? Logout.cshtml.cs
                ??? RegisterConfirmation.cshtml
                ??? RegisterConfirmation.cshtml.cs
```

---

## ?? Página de Registro Personalizada

### Register.cshtml.cs

**Cambios Principales**:

#### 1. InputModel Extendido

```csharp
public class InputModel
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El {0} debe tener al menos {2} y máximo {1} caracteres.", MinimumLength = 2)]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "El apellido es obligatorio")]
    [StringLength(100, ErrorMessage = "El {0} debe tener al menos {2} y máximo {1} caracteres.", MinimumLength = 2)]
    [Display(Name = "Apellido")]
    public string Apellido { get; set; }

    [Required(ErrorMessage = "El correo electrónico es obligatorio")]
    [EmailAddress(ErrorMessage = "El correo electrónico no es válido")]
    [Display(Name = "Correo Electrónico")]
    public string Email { get; set; }

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y máximo {1} caracteres.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirmar Contraseña")]
    [Compare("Password", ErrorMessage = "La contraseña y la confirmación no coinciden.")]
    public string ConfirmPassword { get; set; }
}
```

**Nuevos campos**:
- ? **Nombre** (requerido, 2-100 caracteres)
- ? **Apellido** (requerido, 2-100 caracteres)

#### 2. Asignación de Campos Personalizados

```csharp
public async Task<IActionResult> OnPostAsync(string returnUrl = null)
{
    returnUrl ??= Url.Content("~/");
    
    if (ModelState.IsValid)
    {
        var user = CreateUser();
        
        // Asignar los campos personalizados
        user.Nombre = Input.Nombre;
        user.Apellido = Input.Apellido;

        await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
        await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
        
        var result = await _userManager.CreateAsync(user, Input.Password);
        // ...
    }
}
```

#### 3. Uso de ApplicationUser

```csharp
private readonly SignInManager<ApplicationUser> _signInManager;
private readonly UserManager<ApplicationUser> _userManager;
private readonly IUserStore<ApplicationUser> _userStore;
private readonly IUserEmailStore<ApplicationUser> _emailStore;

private ApplicationUser CreateUser()
{
    try
    {
        return Activator.CreateInstance<ApplicationUser>();
    }
    catch
    {
        throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'...");
    }
}
```

---

## ?? Diseño Personalizado

### Register.cshtml

**Características del diseño**:
- ? **Layout centrado** con card shadow
- ? **Icono de usuario** grande en la parte superior
- ? **Campos en dos columnas** (Nombre | Apellido)
- ? **Validación visual** con mensajes de error
- ? **Gradiente personalizado** en botón principal
- ? **Enlaces útiles** (¿Ya tienes cuenta?)
- ? **Soporte para proveedores externos** (opcional)

**Estructura HTML**:
```html
<div class="container my-5">
    <div class="card shadow-lg">
        <div class="card-body p-5">
            <!-- Icono y título -->
            <div class="text-center mb-4">
                <i class="bi bi-person-plus-fill text-primary"></i>
                <h2>Crear Cuenta</h2>
            </div>

            <!-- Formulario -->
            <form method="post">
                <!-- Nombre y Apellido en fila -->
                <div class="row mb-3">
                    <div class="col-md-6">
                        <input asp-for="Input.Nombre" />
                    </div>
                    <div class="col-md-6">
                        <input asp-for="Input.Apellido" />
                    </div>
                </div>

                <!-- Email, Contraseña, Confirmar -->
                <!-- ... -->

                <button type="submit">Crear Cuenta</button>
            </form>
        </div>
    </div>
</div>
```

### Login.cshtml

**Características**:
- ? **Diseño similar** al registro para consistencia
- ? **Campos:** Email, Contraseña, Recordarme
- ? **Enlaces:** Olvidaste contraseña, Registrarse
- ? **Mensajes traducidos** al español

---

## ?? Configuración de Seguridad

### Políticas de Contraseña

```csharp
options.Password.RequireDigit = true;              // Requiere dígito (0-9)
options.Password.RequireLowercase = true;          // Requiere minúscula (a-z)
options.Password.RequireUppercase = true;          // Requiere mayúscula (A-Z)
options.Password.RequireNonAlphanumeric = false;   // NO requiere símbolos
options.Password.RequiredLength = 6;               // Mínimo 6 caracteres
```

**Ejemplo de contraseña válida**: `Pass123`

### Políticas de Usuario

```csharp
options.User.RequireUniqueEmail = true;            // Emails únicos
```

### Configuración de Login

```csharp
options.SignIn.RequireConfirmedAccount = false;    // NO requiere confirmar email
```

**?? Nota**: Para producción, se recomienda:
- Requerir confirmación de email
- Aumentar longitud mínima a 8 caracteres
- Requerir caracteres especiales
- Implementar autenticación de dos factores

---

## ?? Flujo de Registro

```
1. Usuario navega a /Identity/Account/Register
   ?
2. Completa formulario:
   - Nombre
   - Apellido
   - Email
   - Contraseña
   - Confirmar Contraseña
   ?
3. Validación en cliente (HTML5 + jQuery)
   ?
4. POST a Register.cshtml.cs
   ?
5. Validación en servidor (ModelState)
   ?
6. Crear ApplicationUser:
   - Asignar Nombre y Apellido
   - Asignar Email como Username
   ?
7. UserManager.CreateAsync(user, password)
   ?
8. Si RequireConfirmedAccount = false:
   - SignInAsync automático
   - Redirigir a returnUrl o Home
   ?
9. Usuario autenticado y listo
```

---

## ?? Flujo de Login

```
1. Usuario navega a /Identity/Account/Login
   ?
2. Completa formulario:
   - Email
   - Contraseña
   - Recordarme (opcional)
   ?
3. POST a Login.cshtml.cs
   ?
4. SignInManager.PasswordSignInAsync()
   ?
5. Si éxito:
   - Crear cookie de autenticación
   - Redirigir a returnUrl o Home
   ?
6. Si falla:
   - Mostrar error
   - Recargar formulario
```

---

## ?? Navegación Actualizada

### _Layout.cshtml

**Menú para usuarios NO autenticados**:
```html
<li class="nav-item">
    <a asp-area="Identity" asp-page="/Account/Login">
        <i class="bi bi-box-arrow-in-right"></i> Iniciar Sesión
    </a>
</li>
<li class="nav-item">
    <a asp-area="Identity" asp-page="/Account/Register">
        <i class="bi bi-person-plus"></i> Registrarse
    </a>
</li>
```

**Menú para usuarios autenticados**:
```html
<li class="nav-item dropdown">
    <a class="nav-link dropdown-toggle" href="#" id="navbarDropdown">
        <i class="bi bi-person-circle"></i> @User.Identity.Name
    </a>
    <ul class="dropdown-menu">
        <li><a class="dropdown-item" href="#">Mi Perfil</a></li>
        <li><a asp-controller="Reservas" asp-action="MisReservas">Mis Reservas</a></li>
        <li><hr class="dropdown-divider"></li>
        <li>
            <form asp-area="Identity" asp-page="/Account/Logout" method="post">
                <button type="submit" class="btn btn-link dropdown-item">
                    Cerrar Sesión
                </button>
            </form>
        </li>
    </ul>
</li>
```

---

## ?? Validaciones Implementadas

### En el Cliente (JavaScript + HTML5)

| Campo | Validaciones |
|-------|--------------|
| **Nombre** | Required, Pattern |
| **Apellido** | Required, Pattern |
| **Email** | Required, Email type |
| **Contraseña** | Required, MinLength(6) |
| **Confirmar** | Required, Must match Password |

### En el Servidor (Data Annotations)

```csharp
[Required(ErrorMessage = "...")]
[StringLength(100, MinimumLength = 2, ErrorMessage = "...")]
[EmailAddress(ErrorMessage = "...")]
[DataType(DataType.Password)]
[Compare("Password", ErrorMessage = "...")]
```

**Mensajes personalizados** en español para mejor UX.

---

## ?? Estilos CSS Personalizados

### Gradiente en Botones

```css
.btn-primary {
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    border: none;
}

.btn-primary:hover {
    background: linear-gradient(135deg, #764ba2 0%, #667eea 100%);
    transform: translateY(-2px);
    box-shadow: 0 0.5rem 1rem rgba(0,0,0,.15);
    transition: all 0.3s ease;
}
```

### Cards con Shadow

```css
.card {
    border: none;
    border-radius: 15px;
}
```

### Floating Labels con Iconos

```css
.form-floating > label {
    padding-left: 2rem;
}
```

---

## ?? Comandos Útiles

### Generar Scaffolding de Identity

```bash
cd SoloUnClick.web

dotnet aspnet-codegenerator identity \
  --dbContext SoloUnClick.Infrastructure.Data.AppDbContext \
  --files "Account.Register;Account.Login;Account.Logout;Account.RegisterConfirmation"
```

### Listar todas las páginas disponibles

```bash
dotnet aspnet-codegenerator identity --listFiles
```

### Regenerar una página específica

```bash
dotnet aspnet-codegenerator identity \
  --dbContext SoloUnClick.Infrastructure.Data.AppDbContext \
  --files "Account.Login" \
  --force
```

---

## ?? Problemas Comunes y Soluciones

### 1. AppDbContext Duplicado

**Problema**: El scaffolding crea un nuevo AppDbContext en `Areas/Identity/Data/`

**Solución**: Eliminar el archivo duplicado
```bash
Remove-Item "Areas\Identity\Data\AppDbContext.cs" -Force
```

### 2. IdentityUser en lugar de ApplicationUser

**Problema**: Las páginas generadas usan `IdentityUser`

**Solución**: Modificar manualmente para usar `ApplicationUser`:
```csharp
// Cambiar de:
private readonly SignInManager<IdentityUser> _signInManager;

// A:
private readonly SignInManager<ApplicationUser> _signInManager;
```

### 3. Error de compilación CS1503

**Problema**: "No se puede convertir de 'AppDbContext' a 'AppDbContext'"

**Causa**: Dos clases AppDbContext en namespaces diferentes

**Solución**: Eliminar el AppDbContext generado por scaffolding

### 4. Páginas no se muestran

**Problema**: Las páginas de Identity no aparecen

**Solución**: Verificar que estén agregados en Program.cs:
```csharp
builder.Services.AddRazorPages();
app.MapRazorPages();
```

---

## ?? Recursos Adicionales

### Documentación Oficial

- [ASP.NET Core Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity)
- [Scaffold Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/scaffold-identity)
- [Customize Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/customize-identity-model)

### Páginas de Identity Disponibles

- Account.AccessDenied
- Account.ConfirmEmail
- Account.ConfirmEmailChange
- Account.ExternalLogin
- Account.ForgotPassword
- Account.ForgotPasswordConfirmation
- Account.Lockout
- Account.Login
- Account.LoginWith2fa
- Account.LoginWithRecoveryCode
- Account.Logout
- Account.Manage._ManageNav
- Account.Manage.ChangePassword
- Account.Manage.DeletePersonalData
- Account.Manage.Disable2fa
- Account.Manage.DownloadPersonalData
- Account.Manage.Email
- Account.Manage.EnableAuthenticator
- Account.Manage.ExternalLogins
- Account.Manage.GenerateRecoveryCodes
- Account.Manage.Index
- Account.Manage.PersonalData
- Account.Manage.ResetAuthenticator
- Account.Manage.SetPassword
- Account.Manage.ShowRecoveryCodes
- Account.Manage.TwoFactorAuthentication
- Account.Register
- Account.RegisterConfirmation
- Account.ResendEmailConfirmation
- Account.ResetPassword
- Account.ResetPasswordConfirmation

---

## ? Checklist de Implementación

- [x] Instalar Microsoft.AspNetCore.Identity.UI
- [x] Instalar Microsoft.VisualStudio.Web.CodeGeneration.Design
- [x] Instalar Microsoft.EntityFrameworkCore.Tools
- [x] Configurar AddIdentity<ApplicationUser, IdentityRole> en Program.cs
- [x] Configurar políticas de contraseña
- [x] Configurar cookies de autenticación
- [x] Agregar AddRazorPages() en Program.cs
- [x] Agregar MapRazorPages() en Program.cs
- [x] Generar scaffolding de páginas de Identity
- [x] Modificar Register.cshtml.cs para usar ApplicationUser
- [x] Agregar campos Nombre y Apellido a InputModel
- [x] Asignar campos personalizados en OnPostAsync
- [x] Actualizar Register.cshtml con campos nuevos
- [x] Modificar Login.cshtml.cs para usar ApplicationUser
- [x] Actualizar Login.cshtml con mejor diseño
- [x] Eliminar AppDbContext duplicado
- [x] Actualizar _Layout.cshtml con menú de usuario
- [x] Agregar enlaces de Login/Register
- [x] Agregar dropdown de usuario autenticado
- [x] Traducir mensajes al español
- [x] Aplicar estilos personalizados
- [x] Verificar compilación exitosa
- [x] Probar flujo de registro
- [x] Probar flujo de login

---

## ?? Resultado Final

El sistema de Identity está **completamente integrado y personalizado**, proporcionando:

? **Registro personalizado** con campos Nombre y Apellido  
? **Login funcional** con ApplicationUser  
? **Páginas en español** con mensajes traducidos  
? **Diseño moderno** con Bootstrap 5 y gradientes  
? **Navegación completa** con menú de usuario  
? **Validaciones robustas** en cliente y servidor  
? **Integración perfecta** con el sistema existente  
? **Sin errores de compilación**  

**¡Los usuarios pueden registrarse e iniciar sesión con sus datos completos!** ??

### Próximos Pasos Recomendados

1. **Implementar confirmación de email**
2. **Agregar recuperación de contraseña**
3. **Crear página de perfil de usuario**
4. **Implementar cambio de contraseña**
5. **Agregar autenticación de dos factores (2FA)**
6. **Implementar proveedores externos** (Google, Facebook)
