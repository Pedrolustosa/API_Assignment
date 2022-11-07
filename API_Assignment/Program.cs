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
/// Find by id a assignments
/// </summary>
app.MapGet("/assignments/{id}", async(int id, AppDbContext db) => await db.Assignments.FindAsync(id) is Assignment assignment ? Results.Ok(assignment) : Results.NotFound());


/// <summary>
///  find a assignments if concluded
/// </summary>
app.MapGet("/assignments/concluded", async (AppDbContext db) =>
    {
        var result = await db.Assignments.Where(a => a.IsConcluded == true).ToListAsync();
        return result;
    }
);

/// <summary>
/// update a assignments
/// </summary>
app.MapPut("/assignments/{id}", async (AppDbContext db, int id, Assignment assignments) =>
    {
        var result = await db.Assignments.FindAsync(id);

        if(assignments is null)
        {
            return Results.NotFound();
        }

        assignments.Name = result?.Name; 
        assignments.IsConcluded = result.IsConcluded;
        await db.SaveChangesAsync();
        return Results.NoContent();
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


/// <summary>
/// Remove by id a assignments
/// </summary>
app.MapDelete("/assignments/{id}", async (int id, AppDbContext db) =>
    {
        var result = await db.Assignments.FindAsync(id);

        if (await db.Assignments.FindAsync(id) is Assignment assignment)
        {
            db.Assignments.Remove(assignment);
            await db.SaveChangesAsync();
            return Results.Ok(assignment);
        }
        return Results.NotFound();
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