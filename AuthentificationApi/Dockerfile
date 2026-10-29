# =========================
# Build
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./AuthentificationApi/AuthentificationApi.csproj ./AuthentificationApi/
COPY ./GestionOrdinateurs.Shared/GestionOrdinateurs.Shared.csproj ./GestionOrdinateurs.Shared/
RUN dotnet restore ./AuthentificationApi
RUN dotnet restore ./GestionOrdinateurs.Shared

# Copy the remaining source code
COPY ./AuthentificationApi/ ./AuthentificationApi/
COPY ./GestionOrdinateurs.Shared/ ./GestionOrdinateurs.Shared/
RUN dotnet publish ./AuthentificationApi -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Development pour garder Swagger actif
ENV ASPNETCORE_ENVIRONMENT=Development

# Même port qu'en local (le client Blazor appelle http://localhost:5216)
ENV ASPNETCORE_HTTP_PORTS=5216
EXPOSE 5216

# Define the entry point
ENTRYPOINT ["dotnet", "AuthentificationApi.dll"]
