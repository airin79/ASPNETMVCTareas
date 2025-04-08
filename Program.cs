using Microsoft.EntityFrameworkCore;
using Tareas.Data;
using DinkToPdf;
using DinkToPdf.Contracts;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Runtime.Loader;
using Tareas.Services;

var builder = WebApplication.CreateBuilder(args);

// Cargar DLL nativa
var context = new CustomAssemblyLoadContext();
var dllPath = Path.Combine(Directory.GetCurrentDirectory(), "C:\\Users\\fstra\\source\\repos\\ASPNETMVCTareas\\wwwroot\\DinkToPdf\\lib\\libwkhtmltox.dll");
context.LoadUnmanagedLibrary(dllPath);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Set DbContext connection string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Azure Blob Storage connection
/*builder.Services.AddSingleton(x =>
    new BlobServiceClient(builder.Configuration.GetConnectionString("AzureBlobStorageConnectionString"))
);*/

// Configurar DinkToPdf (usando SynchronizedConverter y PdfTools)
builder.Services.AddSingleton<IConverter, SynchronizedConverter>(sp =>
    new SynchronizedConverter(new PdfTools()));

// Asegurarse de que el ejecutable de wkhtmltopdf sea accesible
var wkHtmlToPdfPath = Path.Combine(builder.Environment.ContentRootPath, "wkhtmltopdf"); // Ajusta la ruta si es necesario
if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    wkHtmlToPdfPath += ".exe";  // Para Windows, agregar la extensión ".exe"
}
/*else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
{
    // Para Linux, debes asegurarte de que wkhtmltopdf esté instalado y accesible
}
else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
{
    // Para Mac, debes asegurarte de que wkhtmltopdf esté instalado
}*/

builder.Services.AddSingleton(wkHtmlToPdfPath); // Registra la ruta de wkhtmltopdf
builder.Services.AddTransient<IEmailService, EmailService>();   // Send email service


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:5000") // improve
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});


var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();




// Configuración de rutas
app.UseRouting();
app.UseCors();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Home}/{id?}");



app.Run();




public class CustomAssemblyLoadContext : AssemblyLoadContext
{
    public IntPtr LoadUnmanagedLibrary(string absolutePath)
    {
        return LoadUnmanagedDllFromPath(absolutePath);
    }

    protected override Assembly Load(AssemblyName assemblyName)
    {
        return null;
    }
}

