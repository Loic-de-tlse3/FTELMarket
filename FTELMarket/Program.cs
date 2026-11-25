using FTELMarket;
using FTELMarket.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Polly;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<FTELMarketDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("FtelMarket")));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services
    // Permet le build du pattern Options
    .AddOptions<FlakySettings>()
    // Va chercher la section "FlakySettings" dans le appsettings.json (ou celui de l'environnement associé à la variable d'env ASPNETCORE_ENVIRONNEMENT)
    .BindConfiguration(nameof(FlakySettings))
    // Permet de valider le modèle avec les annotations telles que [Required]
    .ValidateDataAnnotations()
    //Valide au lancement de l'application
    .ValidateOnStart();

// Option 1 : L'IHttpClientFactory sera récupérer uniquement dans FlakyService
builder.Services.AddHttpClient<FlakyService>((sp, client) =>
{
    var settings = sp.GetRequiredService<IOptions<FlakySettings>>().Value;
    client.BaseAddress = new Uri(settings.BaseAddress);
})
.AddStandardResilienceHandler(options =>
{
    // options.Retry.MaxRetryAttempts = 8;
    // options.Retry.Delay = TimeSpan.FromMilliseconds(500);
    // options.Retry.BackoffType = DelayBackoffType.Exponential;
    // options.Retry.UseJitter = true;

    // // Les timeouts doivent rester cohérents entre eux
    // options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
    // options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(90);

    // // Le circuit breaker doit échantillonner sur au moins 2x AttemptTimeout
    // options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
    // options.CircuitBreaker.FailureRatio = 0.9;
    // options.CircuitBreaker.MinimumThroughput = 20;
});


builder.Services.AddScoped<ClientSyncService>();
builder.Services.AddScoped<ProductSyncService>();
builder.Services.AddScoped<OrderSyncService>();

builder.Host.UseSerilog((context, services, configuration) => configuration
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/sync-.log", rollingInterval: RollingInterval.Day));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
