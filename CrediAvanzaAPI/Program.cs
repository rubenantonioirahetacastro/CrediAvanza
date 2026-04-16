using CrediAvanzaAPI.Helpers;
using CrediAvanzaAPI.Models;
using CrediAvanzaAPI.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.ApplicationInsights.TelemetryConverters;

var builder = WebApplication.CreateBuilder(args);

var applicationInsightsConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.ApplicationInsights(applicationInsightsConnectionString, new TraceTelemetryConverter())
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddScoped<ISolicitudCreditoService, SolicitudCreditoService>();
builder.Services.AddScoped<ICatalogoCodigoService, CatalogoCodigoService>();
builder.Services.AddScoped<ICalendarioService, CalendarioService>();
builder.Services.AddScoped<IFeriadoService, FeriadoService>();
builder.Services.AddScoped<IGastoService, GastoService>();
builder.Services.AddScoped<IAgenciaService, AgenciaService>();
builder.Services.AddScoped<ErrorLogger>();

builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = applicationInsightsConnectionString;
});

builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("Default")
        ?? throw new InvalidOperationException("Connection string 'Default' not found."));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DbNegocioContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("conexion")
    ?? throw new InvalidOperationException("Connection string 'API_CrediAvanzaContext' not found.")));
var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    });
//}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

// deploy inicial


//Scaffold - DbContext "Server=DESKTOP-KTHL7K7\SQLEXPRESS;Database=DbNegocio;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer - OutputDir Models - Context DbNegocioContext - Force
//Server=tcp:crediavanza-server.database.windows.net,1433;Initial Catalog=DbNegocio;Persist Security Info=False;User ID=crediavanza;Password=Pepe1234;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
//Scaffold-DbContext "Server=tcp:crediavanza-server.database.windows.net,1433;Initial Catalog=DbNegocio;Persist Security Info=False;User ID=crediavanza;Password=Pepe1234;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -Context DbNegocioContext -Force