using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using SoloUnClick.Domain.Entities;
using SoloUnClick.Infrastructure.Data;
using SoloUnClick.Infrastructure.Extensions;
using SoloUnClick.web.Extensions;
using SoloUnClick.web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Agregar servicios de Infrastructure (DbContext + UnitOfWork + Repositorios)
builder.Services.AddInfrastructureServices(builder.Configuration);

// Registrar IEmailSender (requerido por Identity)
builder.Services.AddTransient<IEmailSender, EmailSender>();

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

var app = builder.Build();

// Ejecutar migraciones y seed de datos solo fuera de Testing
if (!app.Environment.IsEnvironment("Testing"))
{
    await app.UseDatabaseMigrationAndSeedAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// Mapear Razor Pages para Identity
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

// Hacer Program visible para pruebas de integración
public partial class Program { }
