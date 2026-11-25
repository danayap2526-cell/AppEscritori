using AppEmocions.Api.Data;
using AppEmocions.Api.Data.Dades;
using AppEmocions.Api.Data.Dashboard;
using AppEmocions.Api.Data.Infants;
using AppEmocions.Api.Data.Jocs;
using AppEmocions.Api.Data.Movil;
using AppEmocions.Api.Data.Usuaris;

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

var builder = WebApplication.CreateBuilder(args);

// Infraestructura
builder.Services.AddSingleton<DbConnectionFactory>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Repositorios y Servicios
builder.Services.AddScoped<MovilRepository>();
builder.Services.AddScoped<DashboardRepository>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<InfantsRepository>();
builder.Services.AddScoped<JocsRepository>();
builder.Services.AddScoped<UsuarisRepository>();
builder.Services.AddScoped<DadesRepository>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();
app.Run();