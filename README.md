# 📚 BookAPI

A testable and well-structured REST API built with ASP.NET Core 8 for managing books, users, and reviews. Features include JWT authentication, FluentValidation, centralized error handling, and detailed logging.

---

## 🔧 Tech Stack

- **.NET 8 (ASP.NET Core Web API)**
- **Entity Framework Core 9** (with SQLite + InMemory for tests)
- **JWT Authentication**
- **FluentValidation**
- **xUnit, AutoFixture, FakeItEasy** (for testing)
- **Swagger / OpenAPI**
- **Mapster** (for lightweight object mapping)

---

## 🧱 Project Structure

```
BookAPI/
├── Controllers/              # API controllers (Book, Auth)
├── Identity/                 # Identity logic (models, services, JWT, validation)
├── Infrastructure/           # Data access, EF configs, UoW, DbContext
├── Contracts/                # DTOs for Books & Auth
├── Services/                 # Application services & validators
├── Middleware/               # Request logging, exception handling
├── Exceptions/               # Custom exception classes & handler
├── Program.cs                # Entry point & DI config

tests/
├── UnitTests/                # Unit tests for services, validators, controllers
└── IntegrationTests/         # Integration tests for API endpoints
```

---

## 🚀 Running the App

```bash
dotnet build
dotnet ef database update
dotnet run
```

Swagger UI: [http://localhost:5177/swagger/index.html](http://localhost:5177/swagger/index.html)

---

## ✅ Testing

Run unit and integration tests:

```bash
dotnet test
```

---

## 🔐 Authentication

JWT-based authentication is enabled. After registering via `/api/auth/register`, you can log in at `/api/auth/login` to receive a token.  
Include the token in requests:

```http
Authorization: Bearer {token}
```

---

## 🧪 Example JSON Payloads

### Register

```json
{
  "firstName": "Jane",
  "lastName": "Doe",
  "email": "jane@example.com",
  "password": "P@ssw0rd!"
}
```

### Create Book

```json
{
  "title": "One Hundred Years of Solitude",
  "author": "Gabriel García Márquez"
}
```

---

## 🧠 Features Implemented

- ✅ JWT Authentication
- ✅ Repository + Unit of Work Pattern
- ✅ Global Exception Handling with ProblemDetails
- ✅ FluentValidation
- ✅ Auto-generated Swagger docs
- ✅ Full unit + integration testing coverage

---

## 📄 License

MIT
