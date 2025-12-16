using Client.Blazor;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Notifications.Web.Options;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//notification services
builder.Services.Configure<WebPushOptions>(builder.Configuration.GetSection(WebPushOptions.SectionKey));
// Configurar HttpClient para usar la URL base del entorno
builder.Services.AddScoped(sp => new HttpClient { BaseAddress =  new Uri("https://localhost:7009") });

// Agregar configuración
builder.Services.AddSingleton(builder.Configuration);

await builder.Build().RunAsync();
