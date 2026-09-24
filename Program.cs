using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<PasswordHasher<Member>>();


// tells app to use sqlite and store the database in a file named libaray.db
builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseSqlite("Data Source=library.db"));

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// get every book in the library
app.MapGet("/books", async (LibraryContext db) =>
    await db.Books.ToListAsync());

// get one specific book by its id
app.MapGet("/books/{id}", async (int id, LibraryContext db) =>
    await db.Books.FindAsync(id) is Book book ? Results.Ok(book) : Results.NotFound());

// add a new book to the library
app.MapPost("/books", async (Book book, LibraryContext db) =>
{
    db.Books.Add(book);
    await db.SaveChangesAsync();
    return Results.Created($"/books/{book.Id}", book);
});

// update an existing book's details
app.MapPut("/books/{id}", async (int id, Book updatedBook, LibraryContext db) =>
{
    var book = await db.Books.FindAsync(id);
    if (book is null) return Results.NotFound();

    book.Title = updatedBook.Title;
    book.Author = updatedBook.Author;
    book.Isbn = updatedBook.Isbn;
    book.IsAvailable = updatedBook.IsAvailable;

    await db.SaveChangesAsync();
    return Results.Ok(book);
});

// remove a book from the library
app.MapDelete("/books/{id}", async (int id, LibraryContext db) =>
{
    var book = await db.Books.FindAsync(id);
    if (book is null) return Results.NotFound();

    db.Books.Remove(book);
    await db.SaveChangesAsync();
    return Results.Ok();
});

// Member endpoints

// register a new member, hashing their password before saving it
app.MapPost("/members", async (MemberRegisterRequest request, LibraryContext db, PasswordHasher<Member> hasher) =>
{
    var member = new Member
    {
        Name = request.Name,
        Email = request.Email
    };
    member.PasswordHash = hasher.HashPassword(member, request.Password);

    db.Members.Add(member);
    await db.SaveChangesAsync();
    return Results.Created($"/members/{member.Id}", member);
});

app.MapGet("/members/{id}", async (int id, LibraryContext db) =>
    await db.Members.FindAsync(id) is Member member ? Results.Ok(member) : Results.NotFound());


app.MapDelete("/members/{id}", async (int id, LibraryContext db) =>
{
    var member = await db.Members.FindAsync(id);
    if (member is null) return Results.NotFound();

    db.Members.Remove(member);
    await db.SaveChangesAsync();
    return Results.Ok();
});

// Borrowing endpoints 

app.MapGet("/borrowings", async (LibraryContext db) =>
    await db.Borrowings.ToListAsync());

// checks a book out to a member, leaves returnedAt empty until they bring it back
app.MapPost("/borrowings", async (Borrowing borrowing, LibraryContext db) =>
{
    db.Borrowings.Add(borrowing);
    await db.SaveChangesAsync();
    return Results.Created($"/borrowings/{borrowing.Id}", borrowing);
});

// marks a borrowing as returned by setting the returnedAt timestamp
app.MapPut("/borrowings/{id}/return", async (int id, LibraryContext db) =>
{
    var borrowing = await db.Borrowings.FindAsync(id);
    if (borrowing is null) return Results.NotFound();

    borrowing.ReturnedAt = DateTime.UtcNow;
    await db.SaveChangesAsync();
    return Results.Ok(borrowing);
});

// Review endpoints

app.MapGet("/reviews", async (LibraryContext db) =>
    await db.Reviews.ToListAsync());

app.MapGet("/reviews/book/{bookId}", async (int bookId, LibraryContext db) =>
    await db.Reviews.Where(r => r.BookId == bookId).ToListAsync());

app.MapPost("/reviews", async (Review review, LibraryContext db) =>
{
    db.Reviews.Add(review);
    await db.SaveChangesAsync();
    return Results.Created($"/reviews/{review.Id}", review);
});

app.Run();

// what the client sends when registering, separate from the Member model
// itself, since we never want a plain password sitting on the Member object
record MemberRegisterRequest(string Name, string Email, string Password);