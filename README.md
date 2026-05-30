# ProjectShashtra

A .NET 8 Web API for product management (CRUD) using stored procedures 
and SQL Server for data access.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?logo=microsoftsqlserver)

---

## What this project covers

- Full CRUD operations for product management
- Stored procedure-based data access (no ORM)
- RESTful API with Swagger/OpenAPI documentation
- Clean separation of Controllers, Models, and Data layers

---

## Architecture
ProjectShashtra/
├── Controllers/
│   └── ProductController.cs     # API endpoints
├── Models/
│   └── Product.cs               # Product entity (Id, Name, Category, Price, Stock)
├── Data/
│   └── ProductRepository.cs     # Data access via stored procedures
└── appsettings.json

---

## Tech stack

| Layer | Technology |
|-------|-----------|
| API | ASP.NET Core 8 Web API |
| Language | C# |
| Data access | ADO.NET + Stored Procedures |
| Database | SQL Server |
| Docs | Swagger / OpenAPI |

---

## API endpoints

```http
GET    /api/product          # Get all products
GET    /api/product/{id}     # Get product by ID
POST   /api/product          # Insert new product
PUT    /api/product/{id}     # Update existing product
DELETE /api/product/{id}     # Delete product
```

---

## Database setup

### Products table

```sql
CREATE TABLE dbo.Products (
    product_id     INT IDENTITY(1,1) PRIMARY KEY,
    product_name   NVARCHAR(200)  NOT NULL,
    category       NVARCHAR(100)  NULL,
    price          DECIMAL(18,2)  NOT NULL,
    stock_quantity INT            NOT NULL
);
```

### Stored procedures used

| Procedure | Purpose |
|-----------|---------|
| `usp_GetProducts` | Fetch all products |
| `usp_GetProductsById` | Fetch single product |
| `usp_insertProduct` | Insert new product |
| `usp_updateProduct` | Update existing product |
| `usp_deleteProduct` | Delete product |

---

## Setup & run

### Prerequisites
- .NET 8 SDK
- SQL Server
- Visual Studio 2022 or VS Code

### Steps

```bash
# Clone the repo
git clone https://github.com/NishantDJ/ProjectShashtra.git
cd ProjectShashtra
```

Add your connection string to `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DBCS": "Server=.;Database=YourDb;Trusted_Connection=True;"
  }
}
```

```bash
# Run the project
dotnet run --project ./ProjectShashtra/ProjectShashtra.csproj
```

Swagger UI available at: `https://localhost:{port}/swagger`

---

## Quick API test

```bash
# Insert a product
curl -X POST https://localhost:5001/api/product \
  -H "Content-Type: application/json" \
  -d '{"name":"Widget","category":"Tools","price":19.99,"stock":100}'
```

---

Built by [Nishant Jarare](https://github.com/NishantDJ)
