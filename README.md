# Inventory Management API

REST API for inventory management developed with **.NET 8**, **ASP.NET Core**, **SQL Server**, **Entity Framework Core** and **Dapper**.

The application provides product and category management, inventory movement registration, data persistence, and OAuth2 authentication.

## Table of Contents

- [Overview](#overview)
- [Main Features](#main-features)
- [Technologies](#technologies)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Prerequisites](#prerequisites)
- [Local Development](#local-development)
- [Running with Docker](#running-with-docker)
- [API Documentation](#api-documentation)
- [Authentication](#authentication)
- [Running Unit Tests](#running-unit-tests)
- [Code Coverage](#code-coverage)
- [AI Usage and Architecture Decisions](#ai-usage-and-architecture-decisions)

## Overview

Inventory Management is a RESTful API designed to manage products, categories, and inventory operations.

The solution follows a layered architecture to separate business rules, application logic, infrastructure concerns, and API endpoints.

It uses Entity Framework Core for read operations and Dapper for write operations, combining ORM capabilities with lightweight SQL execution.

## Main Features

- **Product Management:** Create, update, retrieve, list, and deactivate products.
- **Category Management:** Create, update, retrieve, list, and delete categories.
- **Inventory Management:** Register inventory entries and exits.
- **Stock Validation:** Prevent invalid quantities and inventory exits exceeding available stock.
- **Data Persistence:** Store products, categories, and inventory movements in SQL Server.
- **Authentication:** Protect API endpoints using OAuth2 and JWT Bearer authentication.
- **API Documentation:** Interactive API documentation through Swagger UI.

## Technologies

| Technology            | Purpose                       |
| --------------------- | ----------------------------- |
| C#                    | Programming language          |
| .NET 8                | Application framework         |
| ASP.NET Core Web API  | REST API development          |
| Entity Framework Core | Data queries and ORM          |
| Dapper                | Database write operations     |
| SQL Server            | Relational database           |
| OAuth2 / JWT Bearer   | Authentication                |
| Swagger / OpenAPI     | API documentation             |
| xUnit                 | Unit testing                  |
| Moq                   | Dependency mocking            |
| Docker                | Containerization              |
| Docker Compose        | Multi-container orchestration |

## Architecture

The solution is organized into four main layers:

- **Inventory.Domain:** Contains business entities, domain rules, and inventory movement types.
- **Inventory.Application:** Contains use cases, handlers, DTOs, and persistence abstractions.
- **Inventory.Infrastructure:** Implements database access and persistence using Entity Framework Core and Dapper.
- **Inventory.Api:** Exposes REST endpoints, configures dependency injection, authentication, Swagger, and HTTP middleware.

The architecture follows separation of concerns and dependency inversion principles.

### Data Access Strategy

- **Entity Framework Core:** Used for database read operations.
- **Dapper:** Used for database write operations.
- **SQL Server:** Used as the persistent relational database.

Inventory movements are processed through a database transaction to keep stock updates and movement records consistent.

## Project Structure

```text
InventoryManagement/
│
├── Inventory.Api/
│   ├── Controllers/
│   ├── Middleware/
│   └── Program.cs
│
├── Inventory.Application/
│   ├── Abstractions/
│   ├── DTOs/
│   └── Features/
│
├── Inventory.Domain/
│   └── Entities/
│
├── Inventory.Infrastructure/
│   ├── Persistence/
│   └── Repositories/
│
├── Inventory.UnitTests/
│   ├── Domain/
│   └── Application/
│
├── Dockerfile
├── docker-compose.yml
├── InventoryManagement.sln
└── README.md
```

## Prerequisites

### Local development

- .NET 8 SDK
- SQL Server 2022
- Visual Studio 2022 or another compatible .NET IDE

### Containerized execution

- Docker Desktop
- Docker Compose
- Keycloak running on the host machine at port `8080`, configured with the `inventory` realm.

### Local Development

**1. Configure the database connection**

The API uses .NET User Secrets to store the local SQL Server connection string.

From the solution root, run:

```powershell
dotnet user-secrets set "ConnectionStrings:InventoryDb" "Server=localhost,1433;Database=InventoryManagementDb;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True;Encrypt=True;" --project .\Inventory.Api
```

Replace `YOUR_PASSWORD` with your local SQL Server password.

**2. Start the required services**

Ensure SQL Server is running and accessible on port `1433`.

Keycloak must also be running on port `8080`, with the `inventory` realm configured.

**3. Run the API**

From the solution root:

```powershell
dotnet restore
dotnet build
dotnet run --project .\Inventory.Api
```

**4. Access Swagger**

Open the Swagger URL shown in the application output. When running with the Docker Compose configuration, Swagger is available at:

```text
http://localhost:5285/swagger
```

For local execution, use the URL and port reported by ASP.NET Core when the application starts.

**5. Debugging with Visual Studio**

Open `InventoryManagement.sln` in Visual Studio 2022, set `Inventory.Api` as the startup project, select the appropriate launch profile, and press `F5` to start debugging.

## Running with Docker

The solution uses Docker Compose to orchestrate the ASP.NET Core API and SQL Server 2022.

### 1. Clone the repository

```bash
git clone <REPOSITORY_URL>
cd InventoryManagement
```

### 2. Configure environment variables

Create a `.env` file in the solution root by copying `.env.example`.

On PowerShell:

```powershell
Copy-Item .env.example .env
```

Edit `.env` and replace the example SQL Server password with a strong local password.

Example:

```dotenv
SA_PASSWORD=YourStrongLocalPassword123!
SQL_DATABASE=InventoryManagementDb
ASPNETCORE_ENVIRONMENT=Development
```

Do not commit the `.env` file or expose database credentials.

### 3. Create the Docker volume

The SQL Server service uses an external Docker volume named `inventory_sql_data` to persist database files.

Create it before starting the containers:

```bash
docker volume create inventory_sql_data
```

### 4. Start the application

Build the API image and start the containers:

```bash
docker compose up -d --build
```

The API container starts after SQL Server passes its health check.

### 5. Verify running containers

```bash
docker compose ps
```

The expected services are:

- `inventory-sql`: SQL Server 2022.
- `inventory-api`: ASP.NET Core Web API.

### 6. Access Swagger

Once the API is running, open:

```text
http://localhost:5285/swagger
```

### 7. Stop the application

```bash
docker compose down
```

The SQL Server data is stored in the external `inventory_sql_data` volume and is not removed by this command.

To inspect the volume:

```bash
docker volume inspect inventory_sql_data
```

### Important configuration notes

- SQL Server is exposed on port `1433`.
- The API is exposed on port `5285`.
- The API connects to SQL Server using the Docker Compose service name `sql`.
- Keycloak must be accessible from the Docker host through port `8080`.
- The configured Keycloak realm is `inventory`.
- The `.env` file contains local configuration and must remain untracked by Git.

## API Documentation

Swagger UI provides an interactive interface to explore and execute the available API endpoints.

Once the application is running, access:

```text
http://localhost:5285/swagger
```

The API includes endpoints for:

- Products
- Categories
- Inventory movements

Protected endpoints require a valid access token.

## Authentication

The API uses OAuth2 and JWT Bearer authentication.

Keycloak is configured as the identity provider for authentication.

Swagger UI supports the OAuth2 Authorization Code flow with PKCE to obtain an access token and authorize API requests.

## Running Unit Tests

The solution includes unit tests for critical domain and application components.

The tests use:

- **xUnit** for test execution.
- **Moq** for mocking repository dependencies.

Run all unit tests from the solution root:

```bash
dotnet test
```

### Test Results

Current test suite:

- **71 tests passed**
- **0 failed**
- **0 skipped**

## Code Coverage

Code coverage is collected using Coverlet.

Generate the coverage report:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

Generate an HTML report using ReportGenerator:

```bash
reportgenerator "-reports:Inventory.UnitTests/TestResults/*/coverage.cobertura.xml" "-targetdir:CoverageReport" "-reporttypes:Html"
```

Open the generated report:

```bash
start .\CoverageReport\index.html
```

Latest coverage results:

| Metric           |    Result |
| ---------------- | --------: |
| Line coverage    |     97.7% |
| Branch coverage  |      100% |
| Covered lines    | 395 / 404 |
| Covered branches |   56 / 56 |

---

**Project:** Inventory Management API  
**Framework:** .NET 8  
**Database:** SQL Server

## AI Usage and Architecture Decisions

### AI-Assisted Development

ChatGPT and Claude were used as AI-assisted development tools throughout the project.

They supported the development process in the following areas:

- **Requirements Analysis:** Reviewing the assignment requirements and identifying the main functional and technical objectives.
- **Architecture Design:** Exploring architectural approaches, separation of responsibilities, and dependency management.
- **Implementation Support:** Reviewing code, discussing implementation alternatives, and identifying potential improvements.
- **Testing Support:** Assisting with test case design and reviewing critical business rules and edge cases.
- **Documentation:** Supporting the preparation and review of technical documentation.

All AI-generated suggestions were reviewed and evaluated before implementation. Final technical decisions, code modifications, and validation remained under developer responsibility.

### Spec-Driven Development Approach

The development process followed an iterative, requirement-driven approach. The assignment requirements were used as the basis for identifying features, defining responsibilities, and implementing the solution.

Rather than blindly accepting AI-generated recommendations, proposed solutions were evaluated against the project requirements, existing architecture, maintainability, and implementation complexity.

### Key Architecture Decisions

| Decision                                              | Rationale                                                                                |
| ----------------------------------------------------- | ---------------------------------------------------------------------------------------- |
| Layered architecture                                  | Separates domain rules, application use cases, infrastructure, and API responsibilities. |
| Entity Framework Core for reads and Dapper for writes | Follows the assignment requirements while using appropriate data access approaches.      |
| Database transactions for inventory movements         | Maintains consistency between stock updates and movement records.                        |
| OAuth2 with Keycloak                                  | Provides centralized authentication and JWT-based API protection.                        |
| Unit testing with xUnit and Moq                       | Validates critical business rules and application behavior independently.                |
| Docker Compose                                        | Simplifies local deployment and coordinates the API and SQL Server containers.           |

AI tools were used as development assistants, not as substitutes for engineering judgment or validation.
