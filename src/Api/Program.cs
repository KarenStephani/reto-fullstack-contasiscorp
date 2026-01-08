using Contasiscorp.Api.Middleware;
using Contasiscorp.Application.Interfaces;
using Contasiscorp.Infrastructure.Data;
using Contasiscorp.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Contasiscorp API",
        Version = "v1",
        Description = "API para gestión de comprobantes electrónicos"
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(3);
    });
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
});

builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(Contasiscorp.Application.Commands.CrearComprobante.CrearComprobanteCommand).Assembly);
    cfg.AddOpenBehavior(typeof(Contasiscorp.Application.Behaviors.ValidationBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(typeof(Contasiscorp.Application.Validators.CrearComprobanteValidator).Assembly);

builder.Services.AddScoped<IComprobanteRepository, ComprobanteRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.WebHost.UseUrls("http://localhost:5000");

var app = builder.Build();

// Migraciones deshabilitadas - tablas creadas manualmente
// using (var scope = app.Services.CreateScope())
// {
//     var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//     db.Database.Migrate();
// }

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => Results.Ok(new
{
    name = "Contasiscorp API",
    version = "1.0",
    status = "running",
    endpoints = new
    {
        comprobantes = "/api/comprobantes",
        swagger = "/swagger"
    }
}));

app.Run();
