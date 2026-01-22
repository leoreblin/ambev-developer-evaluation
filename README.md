# Developer Evaluation Project (.NET)

## Overview
This repository contains a sales API built with a DDD-inspired structure. The focus is the Sales aggregate and its business rules (discounts, limits, and cancellation) with clean layering and consistent error handling.

## Architecture
- Domain: aggregates, business rules, domain events.
- Application: use cases with MediatR handlers, validation, and event handlers.
- ORM: EF Core mappings, repositories, and migrations.
- WebApi: controllers, request validation, and middleware (ProblemDetails).

## Business Rules
- Quantity based discounts per product:
  - 4-9 items: 10% discount
  - 10-20 items: 20% discount
- Maximum of 20 identical items per product.
- No discount for quantities below 4.
- A cancelled sale cancels all items and has total 0.
- Updating a sale replaces active items by product; missing items are cancelled.

## Running the project
From repository root:

### With Docker (recommended)
1) `docker compose up -d --build`

The application applies EF Core migrations at startup.

### Local runtime
- .NET SDK 8
- PostgreSQL, MongoDB, and Redis running locally
- Update connection strings in `template/backend/src/Ambev.DeveloperEvaluation.WebApi/appsettings.json` if needed

Run:
- `dotnet run --project src/Ambev.DeveloperEvaluation.WebApi`

## Running tests
- `dotnet test template/backend/tests/Ambev.DeveloperEvaluation.Unit/Ambev.DeveloperEvaluation.Unit.csproj`

## Authentication
Default user seeded in Development:
- Email: `user@local.com`
- Password: `default@123`

Use `POST /auth` to get a token, then pass `Authorization: Bearer <token>` to call protected endpoints.

## Notes and decisions
- Error responses follow ProblemDetails via a global exception middleware.
- Domain events are published via MediatR handlers and logged (SaleCreated, SaleModified, SaleCancelled, SaleItemCancelled).
- Sales totals are calculated inside the aggregate based on active (non-cancelled) items.
