[Back to README](../README.md)

## General API Definitions

### Pagination and listing
List endpoints use the following query parameters:

- `pageNumber` (default: 1)
- `pageSize` (default: 10)
- `orderBy` (default: `Title` for products)
- `isDescending` (default: `false`)
- `term` (optional search term)

Example:
```
GET /products?pageNumber=2&pageSize=20&orderBy=Price&isDescending=true&term=beer
```

### Error Handling
The API uses RFC 7807 ProblemDetails.

Example (validation):
```json
{
  "type": "about:blank",
  "title": "Validation Failed",
  "status": 400,
  "detail": "One or more validation errors occurred.",
  "errors": {
    "General": [
      "Quantity must be greater than 0."
    ]
  }
}
```

Example (not found):
```json
{
  "type": "about:blank",
  "title": "Not Found",
  "status": 404,
  "detail": "Sale not found."
}
```

<br>
<div style="display: flex; justify-content: space-between;">
  <a href="./frameworks.md">Previous: Frameworks</a>
  <a href="./products-api.md">Next: Products API</a>
</div>
