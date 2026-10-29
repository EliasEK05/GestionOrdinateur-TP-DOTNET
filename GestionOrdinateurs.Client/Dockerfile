# =========================
# Build
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./GestionOrdinateurs.Client/GestionOrdinateurs.Client.csproj ./GestionOrdinateurs.Client/
RUN dotnet restore ./GestionOrdinateurs.Client

# Copy the remaining source code
COPY ./GestionOrdinateurs.Client/ ./GestionOrdinateurs.Client/
RUN dotnet publish ./GestionOrdinateurs.Client -c Release -o out

# Build runtime image
# Blazor WebAssembly tourne dans le navigateur : nginx sert seulement les fichiers statiques
FROM nginx:alpine AS runtime
COPY --from=build /app/out/wwwroot /usr/share/nginx/html

# Configuration nginx du TP (écoute sur le port 5000)
COPY ./GestionOrdinateurs.Client/nginx.conf /etc/nginx/conf.d/default.conf

# Expose port 5000
EXPOSE 5000
