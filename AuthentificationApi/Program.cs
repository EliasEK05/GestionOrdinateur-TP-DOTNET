var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// HttpClientFactory : client nommé pour appeler GestionOrdinateurs.API
builder.Services.AddHttpClient("UsersApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["GestionOrdinateursApi"]!);
});

// CORS : autorise le client Blazor à appeler cette API
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

//http://localhost:5216/swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Activer CORS
app.UseCors("AllowBlazorClient");

app.MapControllers();

app.Run();
