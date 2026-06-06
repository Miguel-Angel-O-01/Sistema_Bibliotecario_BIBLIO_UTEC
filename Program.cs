using Microsoft.EntityFrameworkCore;
using Sistema_Bibliotecario_BIBLIO_UTEC;
using Sistema_Bibliotecario_BIBLIO_UTEC.Data;
using Sistema_Bibliotecario_BIBLIO_UTEC.Service;
using Sistema_Bibliotecario_BIBLIO_UTEC.Services;

var builder = WebApplication.CreateBuilder(args);


// 1. CONFIGURACIÓN DE SERVICIOS (CONTENEDOR DE DEPENDENCIAS - IoC)

builder.Services.AddControllersWithViews();

// Configuración crítica para las Sesiones HTTP (Requerido por tu carrito)
builder.Services.AddDistributedMemoryCache(); // Almacenamiento en memoria para la sesión
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // El carrito expira tras 30 minutos de inactividad
    options.Cookie.HttpOnly = true;                 // Mitiga ataques XSS protegiendo la cookie de sesión
    options.Cookie.IsEssential = true;               // Obligatorio para que funcione si hay políticas de GDPR
});

// Registro de HttpClient de forma eficiente (Pattern de producción)
builder.Services.AddHttpClient();

// Inyección de Dependencias para las API´s y Servicios comerciales
// Configuración de Entity Framework con SQLite usando la cadena de appsettings.json
builder.Services.AddScoped<CaptchaService>();
builder.Services.AddScoped<GoogleBooksService>();
builder.Services.AddScoped<StripeService>();
builder.Services.AddScoped<BrevoEmailService>();
builder.Services.AddScoped<AlgoliaSearchService>();
builder.Services.AddScoped<GoogleAuthService>();
builder.Services.AddScoped<NytBooksService>();
builder.Services.AddDbContext<BiblioUtecContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));




var app = builder.Build();

// 2. CONFIGURACIÓN DEL PIPELINE DE MIDDLEWARES 

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // HSTS (Seguridad estricta de transporte)
    app.UseHsts();
}

// Comentado temporalmente por tu restricción de no usar HTTPS localmente
// app.UseHttpsRedirection(); 

app.UseStaticFiles();

app.UseRouting();

// ¡CRÍTICO! Habilitar las sesiones HTTP en el ciclo de vida de las peticiones.
// Debe ir obligatoriamente después de UseRouting y antes de UseAuthorization.
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
