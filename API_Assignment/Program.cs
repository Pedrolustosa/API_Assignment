using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("AssignmentDB"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.MapGet("/", () => "Olá Mundo!");
//app.MapGet("frases", async () => await new HttpClient().GetStringAsync("https://ron-swanson-quotes.herokuapp.com/v2/quotes"));


/// <summary>
/// Return a list Assignment
/// </summary>
app.MapGet("/assignments", async (AppDbContext db) =>
    {
        var result = await db.Assignments.ToListAsync();
        return result;
    }
);

/// <summary>
/// Create a Assignment
/// </summary>
app.MapPost("/assignments", async (Assignment assignment, AppDbContext db) =>
    {
        db.Assignments.AddAsync(assignment);
        await db.SaveChangesAsync();
        return Results.Created($"/assignment/{assignment.Id}", assignment);
    }
);

app.Run();

class Assignment
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsConcluded { get; set; }
}

class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Assignment> Assignments => Set<Assignment>();
}