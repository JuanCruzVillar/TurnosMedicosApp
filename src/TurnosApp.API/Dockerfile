# Dockerfile para TurnosApp API
# Multi-stage build para optimizar el tamaño de la imagen

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar archivos de proyecto
COPY ["TurnosApp.API/TurnosApp.API.csproj", "TurnosApp.API/"]
COPY ["TurnosApp.Application/TurnosApp.Application.csproj", "TurnosApp.Application/"]
COPY ["TurnosApp.Contracts/TurnosApp.Contracts.csproj", "TurnosApp.Contracts/"]
COPY ["TurnosApp.Domain/TurnosApp.Domain.csproj", "TurnosApp.Domain/"]
COPY ["TurnosApp.Infrastructure/TurnosApp.Infrastructure.csproj", "TurnosApp.Infrastructure/"]

# Restaurar dependencias
RUN dotnet restore "TurnosApp.API/TurnosApp.API.csproj"

# Copiar el resto del código
COPY . .

# Build
WORKDIR "/src/TurnosApp.API"
RUN dotnet build "TurnosApp.API.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "TurnosApp.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080

# Copiar archivos publicados
COPY --from=publish /app/publish .

# Healthcheck
HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
  CMD curl -f http://localhost:8080/swagger/index.html || exit 1

ENTRYPOINT ["dotnet", "TurnosApp.API.dll"]
