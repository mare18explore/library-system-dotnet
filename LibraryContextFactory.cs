using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

// EF Core's design-time tools (like "dotnet ef migrations add") can't see how
// Program.cs wires everything together, so this gives them a standalone way
// to build a LibraryContext just for generating/running migrations
public class LibraryContextFactory : IDesignTimeDbContextFactory<LibraryContext>
{
    public LibraryContext CreateDbContext(string[] args)
    {
        // same sqlite setup as Program.cs, just built manually here
        // instead of through the app's normal startup
        var optionsBuilder = new DbContextOptionsBuilder<LibraryContext>();
        optionsBuilder.UseSqlite("Data Source=library.db");

        return new LibraryContext(optionsBuilder.Options);
    }
}