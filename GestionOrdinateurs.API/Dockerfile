# =========================
# Build
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./GestionOrdinateurs.API/GestionOrdinateurs.API.csproj ./GestionOrdinateurs.API/
COPY ./GestionOrdinateurs.Shared/GestionOrdinateurs.Shared.csproj ./GestionOrdinateurs.Shared/
RUN dotnet restore ./GestionOrdinateurs.API
RUN dotnet restore ./GestionOrdinateurs.Shared

# Copy the remaining source code
COPY ./GestionOrdinateurs.API/ ./GestionOrdinateurs.API/
COPY ./GestionOrdinateurs.Shared/ ./GestionOrdinateurs.Shared/
RUN dotnet publish ./GestionOrdinateurs.API -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Base SQLite avec les données existantes (ordinateurs, réparations, users)
COPY ./GestionOrdinateurs.API/parcinfo.db ./

# Development pour garder Swagger actif
ENV ASPNETCORE_ENVIRONMENT=Development

# Même port qu'en local (appelé par AuthentificationApi et la Gateway)
ENV ASPNETCORE_HTTP_PORTS=5151
EXPOSE 5151

# Define the entry point
ENTRYPOINT ["dotnet", "GestionOrdinateurs.API.dll"]
