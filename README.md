# API-inventory-management-
A swagger backend API developed using ASP.NET Core .NET 8 Web API for an inventory management application. The API supports user registration, login and inventory product management, with data stored in Microsoft SQL Server 2019.

## Technologies
- C#
- ASP.NET Core .NET 8 Web API
- Microsoft SQL Server 2019
- Entity Framework Core (if used)
- RESTful API architecture

## Features
- User registration
- User authentication and login
- Create inventory product records
- Retrieve and browse inventory products
- SQL Server database integration
- HTTP endpoints for communication with the Angular frontend

## Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Microsoft SQL Server 2019
- SQL Server Management Studio (SSMS), recommended

## Getting Started

### 1. Clone the repository

```bash
git clone YOUR_REPOSITORY_URL
cd YOUR_PROJECT_FOLDER
```

### 2. Configure the database

Configure the SQL Server connection string in `appsettings.json`, user secrets or environment variables, depending on the project's configuration.

Do not commit database passwords or other sensitive credentials.

### 3. Restore dependencies

```bash
dotnet restore
```

### 4. Set up the database

If the project uses Entity Framework Core migrations, apply them using:

```bash
dotnet ef database update
```

This requires the Entity Framework Core tools and a configured database connection. Follow the project's existing database setup instructions if it uses a different approach.

### 5. Run the API

```bash
dotnet run
```

The terminal will display the local URLs where the API is running.

## API Documentation

If Swagger is configured and enabled, open the Swagger UI URL provided by the application to explore and test the available endpoints.

## Integration
The API is designed to integrate with an Angular frontend, providing backend services for authentication and inventory product management.

## Security
Database credentials and other secrets should be stored securely and excluded from version control. Authentication and authorisation should be configured according to the application's requirements.

## Purpose
This project demonstrates backend development using .NET 8 Web API, database integration, RESTful service design and authentication functionality.
