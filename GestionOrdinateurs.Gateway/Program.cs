var builder = WebApplication.CreateBuilder(args);

// YARP : les routes et destinations sont lues dans appsettings.json (section ReverseProxy)
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// CORS : le client Blazor appelle maintenant la Gateway
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins(
                  "http://localhost:5282", // client Blazor lancé avec dotnet run
                  "http://localhost:5000") // client Blazor sous Docker (nginx)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

//Activer CORS
app.UseCors("AllowBlazorClient");

// Redirige les appels vers les APIs
app.MapReverseProxy();

app.Run();
