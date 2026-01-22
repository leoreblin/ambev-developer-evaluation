[Back to README](../README.md)

### Users

#### POST /users
- Description: Add a new user
- Request Body:
  ```json
  {
    "username": "string",
    "password": "string",
    "phone": "string",
    "email": "string",
    "status": "string (enum: Active, Inactive, Suspended)",
    "role": "string (enum: Customer, Manager, Admin)"
  }
  ```
- Response:
  ```json
  {
    "success": true,
    "message": "User created successfully",
    "data": {
      "id": "guid",
      "name": "string",
      "email": "string",
      "phone": "string",
      "status": "string (enum: Active, Inactive, Suspended)",
      "role": "string (enum: Customer, Manager, Admin)"
    }
  }
  ```

#### GET /users/{id}
- Description: Retrieve a specific user by ID
- Path Parameters:
  - `id`: User ID
- Response:
  ```json
  {
    "success": true,
    "message": "User retrieved successfully",
    "data": {
      "id": "guid",
      "name": "string",
      "email": "string",
      "phone": "string",
      "status": "string (enum: Active, Inactive, Suspended)",
      "role": "string (enum: Customer, Manager, Admin)"
    }
  }
  ```

#### DELETE /users/{id}
- Description: Delete a specific user
- Path Parameters:
  - `id`: User ID
- Response:
  ```json
  {
    "success": true,
    "message": "User deleted successfully"
  }
  ```

<br/>
<div style="display: flex; justify-content: space-between;">
  <a href="./carts-api.md">Previous: Carts API</a>
  <a href="./auth-api.md">Next: Auth API</a>
</div>
