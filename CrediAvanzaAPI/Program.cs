using CrediAvanzaAPI.Models;
using CrediAvanzaAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<ISolicitudCreditoService, SolicitudCreditoService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DbNegocioContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("conexion")
    ?? throw new InvalidOperationException("Connection string 'API_CrediAvanzaContext' not found.")));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// deploy inicial
