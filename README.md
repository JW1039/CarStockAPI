# Car Stock Management API

## Prerequisites
Before running the project, please ensure that the following prerequisites are installed:
- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)

### Dependencies
- **Dapper**: Used for database queries.
- **Swashbuckle/Swagger**: Generates the API documentation.



## Setup Instructions

### 1. Clone the Repository
Clone the repository to your local machine:
```bash
git clone https://github.com/your-repository/CarStockManagementAPI.git
```

### 2. Build the Project
Navigate to the project directory and run the following command to build the project:
```
dotnet build
```

### 4. Run the Project
To run the API, use the following command:
```
dotnet run
```

The project will now be running at `http://localhost:7037`

### 5. Access Swagger UI
Once the application is running, you can view the API documentation and test endpoints using Swagger:
```
https://localhost:7037/swagger
```

## Authentication
Available user accounts are:
Johns-Auto
Premium-Cars
Budget-Motors
**The password for all of these is MD5 hashed for simplicity and is set to:**
password
*For all users.*

This project uses **cookie-based authentication**. You must first log in as a dealer by sending a `POST` request to the `/api/dealers/login` endpoint. Upon successful login, a cookie will be set, and it will be used for all subsequent requests to authorized endpoints.

Example login request:
```bash
curl -X POST https://localhost:5001/api/dealers/login \
-H "Content-Type: application/json" \
-d '{
  "name": "Johns-Auto",
  "password": "password"
}'
```

## Database
The database is SQLite stored in [/Data/cars.db](./Data/cars.db)