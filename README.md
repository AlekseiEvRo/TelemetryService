# Telemetry & Analytics Backend Service

Backend service for ingesting, storing and analyzing telemetry data from devices.

The project demonstrates a typical backend architecture with REST API, batch data ingestion, analytics aggregation and containerized infrastructure.

## Features
- Device management (CRUD)
- Batch telemetry ingestion
- Daily analytics (avg / min / max, counts, ratios)
- RESTful API
- PostgreSQL storage
- Health checks
- Docker & Docker Compose

## Tech Stack
- C#
- .NET 8
- ASP.NET Core Web API
- Entity Framework Core (Code First, Migrations)
- PostgreSQL
- Docker, Docker Compose
- Swagger / OpenAPI

## Architecture
The project follows a layered architecture:
- Domain – entities and enums
- Application – DTOs and business logic
- Infrastructure – database access (EF Core)
- WebApi – REST API, controllers, middleware

## API Overview
- `POST /api/devices` – create device
- `GET /api/devices/{id}` – get device by id
- `POST /api/telemetry` – batch telemetry ingestion
- `GET /api/devices/{id}/analytics/daily` – daily analytics
- `/health` – health check

## Running the project

### Using Docker
```bash
docker compose up --build
