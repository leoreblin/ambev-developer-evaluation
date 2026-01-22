[Back to README](../README.md)


### Authentication

#### POST /auth
- Description: Authenticate a user
- Request Body:
  ```json
  {
    "email": "string",
    "password": "string"
  }
  ```
- Response: 
  ```json
  {
    "success": true,
    "message": "User authenticated successfully",
    "data": {
      "token": "string"
    }
  }
  ```

<br/>
<div style="display: flex; justify-content: space-between;">
  <a href="./users-api.md">Previous: Users API</a>
  <a href="./sales-api.md">Next: Sales API</a>
</div>
