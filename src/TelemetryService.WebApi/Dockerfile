FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/TelemetryService.Domain/TelemetryService.Domain.csproj src/TelemetryService.Domain/
COPY src/TelemetryService.Application/TelemetryService.Application.csproj src/TelemetryService.Application/
COPY src/TelemetryService.Infrastructure/TelemetryService.Infrastructure.csproj src/TelemetryService.Infrastructure/
COPY src/TelemetryService.WebApi/TelemetryService.WebApi.csproj src/TelemetryService.WebApi/

RUN dotnet restore src/TelemetryService.WebApi/TelemetryService.WebApi.csproj

COPY . .

RUN dotnet publish src/TelemetryService.WebApi/TelemetryService.WebApi.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "TelemetryService.WebApi.dll"]
