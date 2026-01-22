[Back to README](../README.md)

### Products

#### GET /products
- Description: Retrieve a paginated list of products
- Query Parameters:
  - `pageNumber` (optional): Page number for pagination (default: 1)
  - `pageSize` (optional): Number of items per page (default: 10)
  - `orderBy` (optional): Property name to order by (default: `Title`)
  - `isDescending` (optional): Sort descending (`true`/`false`)
  - `term` (optional): Search term for text search
- Response: 
  ```json
  {
    "success": true,
    "data": [
      {
        "id": "guid",
        "title": "string",
        "price": 0.0,
        "description": "string",
        "category": "string",
        "imageUrl": "string",
        "rating": {
          "rate": 0.0,
          "count": 0
        }
      }
    ],
    "currentPage": 1,
    "totalPages": 1,
    "totalCount": 1,
    "hasPrevious": false,
    "hasNext": false
  }
  ```

<br>
<div style="display: flex; justify-content: space-between;">
  <a href="./general-api.md">Previous: General API</a>
  <a href="./carts-api.md">Next: Carts API</a>
</div>
