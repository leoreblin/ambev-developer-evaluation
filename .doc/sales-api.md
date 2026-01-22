[Back to README](../README.md)

### Sales

#### GET /sales/{id}
- Description: Retrieve a sale by its identifier
- Response:
  ```json
  {
    "success": true,
    "data": {
      "id": "guid",
      "number": "string",
      "occurredAt": "2025-01-01T12:00:00Z",
      "customerId": "guid",
      "customerName": "string",
      "branchId": "guid",
      "branchName": "string",
      "totalAmount": 100.0,
      "isCancelled": false,
      "items": [
        {
          "itemId": "guid",
          "productId": "guid",
          "productName": "string",
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

#### GET /sales/customers/me
- Description: Retrieve sales for the authenticated customer (paginated)
- Query Parameters:
  - `pageNumber` (optional): Page number for pagination (default: 1)
  - `pageSize` (optional): Number of items per page (default: 10)
  - `saleNumber` (optional): Filter by sale number

#### POST /sales
- Description: Create a new sale
- Request Body:
  ```json
  {
    "customerId": "guid",
    "branchId": "guid",
    "items": [
      {
        "productId": "guid",
        "quantity": 4,
        "unitPrice": 10.0
      }
    ]
  }
  ```
- Response:
  ```json
  {
    "success": true,
    "message": "Sale created successfully",
    "data": "guid"
  }
  ```

#### POST /sales/from-cart
- Description: Create a new sale from the customer's cart
- Request Body:
  ```json
  {
    "customerId": "guid",
    "branchId": "guid"
  }
  ```
- Response:
  ```json
  {
    "success": true,
    "message": "Sale created successfully",
    "data": "guid"
  }
  ```

#### PUT /sales/{id}
- Description: Replace the sale items (missing items are cancelled)
- Request Body:
  ```json
  {
    "items": [
      {
        "productId": "guid",
        "quantity": 5,
        "unitPrice": 10.0
      }
    ]
  }
  ```
- Response: `204 No Content`

#### PATCH /sales/{id}
- Description: Cancel a sale
- Response: `204 No Content`

#### PATCH /sales/{id}/items/{itemId}
- Description: Cancel a specific sale item
- Response: `204 No Content`

<br/>
<div style="display: flex; justify-content: space-between;">
  <a href="./auth-api.md">Previous: Auth API</a>
  <a href="./project-structure.md">Next: Project Structure</a>
</div>
