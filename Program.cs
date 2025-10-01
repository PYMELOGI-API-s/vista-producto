using myapp.Components;
using BlazorApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register ProductService
builder.Services.AddScoped<IProductService, ProductService>();

// Register HttpClient for external API access
// Configurar HttpClient para el servicio de productos
builder.Services.AddHttpClient<IProductService, ProductService>(client =>
{
    client.BaseAddress = new Uri("https://api-producto-07d2.onrender.com/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("User-Agent", "Blazor-Client");
    client.Timeout = TimeSpan.FromSeconds(30);
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
});

// Configurar HttpClient genérico para cargas de archivos
builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri("https://api-producto-07d2.onrender.com/");
    client.DefaultRequestHeaders.Add("User-Agent", "Blazor-Client");
    client.Timeout = TimeSpan.FromSeconds(60); // Timeout más largo para cargas de archivos
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

// Habilitar archivos estáticos (necesario para servir imágenes desde wwwroot)
app.UseStaticFiles();

app.UseHttpsRedirection();


app.UseAntiforgery();

app.UseStaticFiles();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
