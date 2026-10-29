# =========================
# Build
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers (la Gateway ne dépend pas de Shared)
COPY ./GestionOrdinateurs.Gateway/GestionOrdinateurs.Gateway.csproj ./GestionOrdinateurs.Gateway/
RUN dotnet restore ./GestionOrdinateurs.Gateway

# Copy the remaining source code
COPY ./GestionOrdinateurs.Gateway/ ./GestionOrdinateurs.Gateway/
RUN dotnet publish ./GestionOrdinateurs.Gateway -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Set environment variables if needed (e.g., ASPNETCORE_ENVIRONMENT)
ENV ASPNETCORE_ENVIRONMENT=Development

# Port 5010 demandé par le TP précédent (le client Blazor appelle http://localhost:5010)
ENV ASPNETCORE_HTTP_PORTS=5010
EXPOSE 5010

# Define the entry point
ENTRYPOINT ["dotnet", "GestionOrdinateurs.Gateway.dll"]
