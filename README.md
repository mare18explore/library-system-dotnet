Library System (.NET)
A C#/ASP.NET Core port of my original Python Library System CLI, rebuilt as a Web API with Entity Framework Core. I built this specifically to get real, hands-on experience with the .NET stack, since C#/.NET kept showing up as a requirement in job postings I was applying to.
Original Python version: https://github.com/mare18explore/library-system-cli
What it does
Same core idea as the original: manage books, members, borrowing, and reviews for a library, now exposed as a REST API instead of a command-line tool.
Features
Full CRUD for books: add, view, update, and remove titles from the catalog
Member registration with secure password hashing, no plain text passwords stored anywhere
Borrowing system that tracks when a book was checked out and lets it be marked as returned
Reviews tied to specific books, with an endpoint to pull all reviews for one title
How it's built
API: ASP.NET Core Web API using the minimal API style (endpoints defined directly in Program.cs rather than separate controller classes).
Database: SQLite, accessed through Entity Framework Core instead of raw SQL. Four related tables (Books, Members, Borrowings, Reviews), matching the schema from the original Python version, with foreign key relationships between them.
Migrations: schema changes are tracked through EF Core migrations rather than hand-written SQL, so the database structure has a real, versioned history from InitialCreate through adding the Member/Borrowing/Review tables.
Authentication: passwords are hashed using ASP.NET Core's built-in PasswordHasher (PBKDF2), the .NET equivalent of what bcrypt did in the Python version. A separate request type keeps the plain password from ever touching the actual Member model.
What I learned porting this
The original Python version used raw SQL directly, so the biggest shift was learning to think in terms of Entity Framework's model-first approach instead, defining C# classes and letting EF Core handle the actual table creation and queries. Debugging the design-time DbContext factory issue with the EF CLI tools and getting used to C#'s top-level statement rules (where type declarations have to come after executable code in Program.cs) were the two things that took the most trial and error.
Run locally
bash
dotnet restore
dotnet ef database update
dotnet run
The API will be available at http://localhost:5245 (or whatever port your local setup assigns). Endpoints:
GET    /books
GET    /books/{id}
POST   /books
PUT    /books/{id}
DELETE /books/{id}

GET    /members/{id}
POST   /members

GET    /borrowings
POST   /borrowings
PUT    /borrowings/{id}/return

GET    /reviews
GET    /reviews/book/{bookId}
POST   /reviews
