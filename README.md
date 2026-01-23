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
- PostgreSQL (sales + users/branches)
- MongoDB (product catalog)
- Redis (cart storage)
- Update connection strings in `src/Ambev.DeveloperEvaluation.WebApi/appsettings.json` if needed

Run:
- `dotnet run --project src/Ambev.DeveloperEvaluation.WebApi`

### Swagger / OpenAPI
In Development, Swagger UI is enabled:
- `http://localhost:8080/swagger` (Docker)

## Running tests
- `dotnet test tests/Ambev.DeveloperEvaluation.Unit/Ambev.DeveloperEvaluation.Unit.csproj`
- `dotnet test tests/Ambev.DeveloperEvaluation.Integration/Ambev.DeveloperEvaluation.Integration.csproj`
- `dotnet test tests/Ambev.DeveloperEvaluation.Functional/Ambev.DeveloperEvaluation.Functional.csproj`

## Authentication
Default user seeded in Development:
- Email: `user@local.com`
- Password: `default@123`

Use `POST /auth` to get a token, then pass `Authorization: Bearer <token>` to call protected endpoints.

## API Contract (summary)
Base URL: `http://localhost:8080`

### Auth
- `POST /auth` (login)

Request:
```json
// Customer
{  
  "email": "customer@local.com",
  "password": "default@123"
}
// Admin
{  
  "email": "admin@local.com",
  "password": "default@123"
}
```

Response:
```json
{
  "success": true,
  "message": "User authenticated successfully",
  "data": {
    "token": "string"
  }
}
```

### Sales
- `GET /sales/{id}` (get by id)
- `GET /sales/customers/me?pageNumber=1&pageSize=10&saleNumber=` (list by customer from JWT `nameid`)
- `POST /sales` (create)
- `POST /sales/from-cart` (create from cart)
- `PUT /sales/{id}` (replace items)
- `PATCH /sales/{id}` (cancel sale)
- `PATCH /sales/{id}/items/{itemId}` (cancel item)

Create request:
```json
{
  "customerId": "guid",
  "branchId": "guid",
  "items": [
    {
      "productId": "guid",
      "quantity": 10,
      "unitPrice": 12.5
    }
  ]
}
```

Response (GET by id):
```json
{
  "success": true,
  "data": {
    "id": "guid",
    "number": "SALE-20250101-1200000123",
    "occurredAt": "2025-01-01T12:00:00Z",
    "customerId": "guid",
    "customerName": "Default User",
    "branchId": "guid",
    "branchName": "Ambev Branch I",
    "totalAmount": 100.00,
    "isCancelled": false,
    "items": [
      {
        "itemId": "guid",
        "productId": "guid",
        "productName": "Product title",
        "itemQuantity": 10,
        "itemUnitPrice": 12.5,
        "itemDiscount": 0.2,
        "itemTotal": 100.0,
        "isCancelled": false
      }
    ]
  }
}
```

### Products
- `GET /products?pageNumber=1&pageSize=10&orderBy=Title&isDescending=false&term=`

### Carts
- `GET /carts/{customerId}`
- `POST /carts/{customerId}`
- `DELETE /carts/{customerId}`
- `DELETE /carts/{customerId}/remove/{productId}`

### Users
- `POST /users`
- `GET /users/{id}`
- `DELETE /users/{id}`

### Branches
- `GET /branches`

### Health
- `GET /health`
- `GET /health/live`
- `GET /health/ready`

## External Identities + denormalization
Sales persist external identities with descriptive fields to preserve historical context:
- Sale stores `CustomerId` + `CustomerName` and `BranchId` + `BranchName`.
- Sale items store `ProductId` + `ProductName`.

## Error responses
Errors follow RFC 7807 `application/problem+json` via a global middleware.
Example (validation):
```json
{
  "type": "about:blank",
  "title": "Validation Failed",
  "status": 400,
  "detail": "One or more validation errors occurred.",
  "errors": {
    "General": [
      "Product ID is required."
    ]
  }
}
```

## API docs and Postman
- Detailed docs: `.doc/general-api.md`, `.doc/products-api.md`, `.doc/carts-api.md`, `.doc/users-api.md`, `.doc/auth-api.md`, `.doc/sales-api.md`
- Postman collection: `postman/ambev-developer-evaluation.postman_collection.json`

## Notes and decisions
- Error responses follow ProblemDetails via a global exception middleware.
- Domain events are published via MediatR handlers and logged (SaleCreated, SaleModified, SaleCancelled, SaleItemCancelled).
- Sales totals are calculated inside the aggregate based on active (non-cancelled) items.
- MongoDB is used for the product catalog; Redis stores carts to enable checkout via `/sales/from-cart`.
