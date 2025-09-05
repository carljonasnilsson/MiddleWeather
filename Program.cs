using WeatherAggregatorDemo.Services;

var builder = WebApplication.CreateBuilder(args);

// Lägg till services för MVC och API
builder.Services.AddControllersWithViews();

// Lägg till HttpClient
builder.Services.AddHttpClient();

// Registrera LocationService
builder.Services.AddTransient<LocationService>();

// Registrera weather providers
builder.Services.AddTransient<IWeatherProvider, SmhiWeatherService>();
builder.Services.AddTransient<IWeatherProvider, MetNoWeatherService>();
builder.Services.AddTransient<IWeatherProvider, OpenWeatherMapService>();

// Registrera aggregator
builder.Services.AddTransient<WeatherDataAggregator>();

var app = builder.Build();

// Configure pipeline
app.UseHttpsRedirection();
app.UseStaticFiles(); // För CSS och JS
app.UseRouting();
app.UseAuthorization();

// MVC routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// API routing
app.MapControllers();

app.Run();
