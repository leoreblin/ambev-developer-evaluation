[Back to README](../README.md)

### Carts

#### GET /carts/{customerId}
- Description: Retrieve the customer's cart
- Response:
  ```json
  {
    "success": true,
    "data": {
      "userId": "guid",
      "items": [
        {
          "product": {
            "id": "guid",
            "title": "string",
            "price": 0.0
          },
          "quantity": 1
        }
      ]
    }
  }
  ```

#### POST /carts/{customerId}
- Description: Add a product to the customer's cart
- Request Body:
  ```json
  {
    "productId": "guid",
    "quantity": 2
  }
  ```
- Response: `204 No Content`

#### DELETE /carts/{customerId}
- Description: Clears the customer's cart
- Response: `204 No Content`

#### DELETE /carts/{customerId}/remove/{productId}
- Description: Removes a product from the cart
- Response: `204 No Content`


<br>
<div style="display: flex; justify-content: space-between;">
  <a href="./products-api.md">Previous: Products API</a>
  <a href="./users-api.md">Next: Users API</a>
</div>
