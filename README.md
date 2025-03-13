# Book Management System API

A RESTful API for managing books, authors, publishers, categories, and reviews. Built with .NET Core 8.0.

## Features

- RESTful API endpoints for all entities
- JWT Authentication and Authorization
- Role-based access control
- Entity Framework Core with SQLite database
- Repository pattern and service layer architecture
- Swagger API documentation
- Dockerization support
- CI/CD with GitHub Actions
- Health checks for monitoring

## Technologies Used

- .NET Core 8.0
- Entity Framework Core
- ASP.NET Core Identity
- JWT Authentication
- Swagger/OpenAPI
- Docker
- GitHub Actions
- SQLite

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- Visual Studio, VS Code, or your preferred IDE
- Docker (optional, for containerization)

### Installation

1. Clone the repository
   ```
   git clone https://github.com/yourusername/BookManagementSystem.git
   ```

2. Navigate to the project directory
   ```
   cd BookManagementSystem
   ```

3. Restore dependencies
   ```
   dotnet restore
   ```

4. Set up environment variables
   ```
   export DATABASE_CONNECTION_STRING="Data Source=books.db"
   export JWT_SECRET_KEY="your-secret-key-here-at-least-32-characters-long"
   ```

5. Run migrations
   ```
   dotnet ef database update
   ```

6. Run the application
   ```
   dotnet run
   ```

7. Access the API at http://localhost:5055
   The Swagger documentation will be available at the root URL.

### Docker Support

Build and run with Docker:

```
docker build -t book-management-api .
docker run -p 8080:8080 \
  -e DATABASE_CONNECTION_STRING="Data Source=books.db" \
  -e JWT_SECRET_KEY="your-secret-key-here" \
  book-management-api
```

Or use Docker Compose:

```
docker-compose up
```

## API Endpoints

### Authentication

- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Login and get JWT token
- `POST /api/auth/refresh` - Refresh an expired JWT token
- `POST /api/auth/logout` - Logout by invalidating refresh token

### Books

- `GET /api/Books` - Get all books
- `GET /api/Books/{id}` - Get a specific book
- `POST /api/Books` - Create a new book (Admin only)
- `PUT /api/Books/{id}` - Update a book (Admin only)
- `DELETE /api/Books/{id}` - Delete a book (Admin only)

### Authors

- `GET /api/Author` - Get all authors
- `GET /api/Author/{id}` - Get a specific author
- `POST /api/Author` - Create a new author (Admin only)
- `PUT /api/Author/{id}` - Update an author (Admin only)
- `DELETE /api/Author/{id}` - Delete an author (Admin only)

### Categories

- `GET /api/Category` - Get all categories
- `GET /api/Category/{id}` - Get a specific category
- `POST /api/Category` - Create a new category (Admin only)
- `PUT /api/Category/{id}` - Update a category (Admin only)
- `DELETE /api/Category/{id}` - Delete a category (Admin only)

### Publishers

- `GET /api/Publisher` - Get all publishers
- `GET /api/Publisher/{id}` - Get a specific publisher
- `POST /api/Publisher` - Create a new publisher (Admin only)
- `PUT /api/Publisher/{id}` - Update a publisher (Admin only)
- `DELETE /api/Publisher/{id}` - Delete a publisher (Admin only)

### Reviews

- `GET /api/Review` - Get all reviews
- `GET /api/Review/{id}` - Get a specific review
- `POST /api/Review` - Create a new review
- `PUT /api/Review/{id}` - Update a review
- `DELETE /api/Review/{id}` - Delete a review

## Project Structure

- `Controllers/` - API controllers
- `Models/` - Data models
- `Data/` - Database context and migrations
- `Repositories/` - Repository pattern implementation
- `Services/` - Business logic layer
- `Identity/` - Authentication and authorization
- `Middleware/` - Custom middleware components
- `DTO/` - Data Transfer Objects

## Contributing

1. Create a feature branch from `develop`
2. Make your changes
3. Submit a pull request to `develop`

## Branching Strategy

This project follows a Git Flow branching strategy:

- `main` - Production-ready code
- `develop` - Development branch (main branch for day-to-day development)
- `feature/xxx` - New features (e.g., feature/add-book-reviews)
- `bugfix/xxx` - Bug fixes (e.g., bugfix/fix-auth-issue)
- `release/x.x.x` - Release preparation

## Commit Message Guidelines

Follow the Conventional Commits specification:

- `feat`: A new feature
- `fix`: A bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting, etc)
- `refactor`: Code refactoring
- `test`: Adding or updating tests
- `chore`: Build process or auxiliary tool changes

Examples:
- `feat: add book search functionality`
- `fix: resolve JWT token validation issue`
- `docs: update API documentation`
- `test: add unit tests for book service`

## License

This project is licensed under the MIT License - see the LICENSE file for details.
