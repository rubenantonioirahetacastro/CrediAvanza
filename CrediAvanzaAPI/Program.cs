using CrediAvanzaAPI.Helpers;
using CrediAvanzaAPI.Models;
using CrediAvanzaAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<ISolicitudCreditoService, SolicitudCreditoService>();
builder.Services.AddScoped<ICatalogoCodigoService, CatalogoCodigoService>();
builder.Services.AddScoped<ICalendarioService, CalendarioService>();
builder.Services.AddScoped<IFeriadoService, FeriadoService>();
builder.Services.AddScoped<IGastoService, GastoService>();
builder.Services.AddScoped<ErrorLogger>();

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
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// deploy inicial

//Scaffold - DbContext "Server=crediavanzasv.database.windows.net;Database=DbNegocio;User Id=CloudAdmin;Password=Pepe2024;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer - OutputDir Models - Context DbNegocioContext - Force
//Scaffold - DbContext "Server=DESKTOP-KTHL7K7\SQLEXPRESS;Database=DbNegocio;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer - OutputDir Models - Context DbNegocioContext - Force