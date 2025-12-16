using NotifyManager;
using NotifyManager.Options;
using NotifyManager.Rest.Mappings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddNotifyManagerCore(huboptions => builder.Configuration.GetSection(AzureNotificationHubOptions.SectionKey).Bind(huboptions));
builder.Services.AddCors(options => 
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseHttpsRedirection();
app.UseCors();
app.UseNotifyManagerEndpoints();

app.Run();
