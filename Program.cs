using Microsoft.EntityFrameworkCore;
using TodoApi; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(); 


// CORS - פתיחת הרשאות כללית עבור http://localhost:3000
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// הגדרת DbContext עם חיבור ל-MySQL
var connectionString = builder.Configuration.GetConnectionString("ToDoDB");

builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(connectionString,
        new MySqlServerVersion(new Version(8, 0, 36)),
        mySqlOptionsAction: options => options.EnableRetryOnFailure()));

// Swagger - הגדרת SwaggerGen
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); 
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API v1");
    });
}

app.UseHttpsRedirection();

// Endpoints לניהול משימות
app.MapGet("/", () => "ברוכה הבאה");


app.MapGet("/tasks", async (ToDoDbContext db) =>
{
    return await db.Tasks.ToListAsync();
});


app.MapPost("/tasks", async (ToDoDbContext db, HttpContext context) =>
{
    // קוראים את ה-JSON מגוף הבקשה
    var item = await context.Request.ReadFromJsonAsync<Item>();

    if (item == null || string.IsNullOrWhiteSpace(item.Name))
    {
        return Results.BadRequest("Task name cannot be empty.");
    }

    db.Tasks.Add(item);
    await db.SaveChangesAsync();
    return Results.Created($"/tasks/{item.Id}", item);
});

app.MapPut("/tasks/{id}", async (ToDoDbContext db, int id, Item updatedItem) =>
{
    if (string.IsNullOrWhiteSpace(updatedItem.Name))
        return Results.BadRequest("Task name cannot be empty.");

    var item = await db.Tasks.FindAsync(id);
    if (item == null) return Results.NotFound();

    item.Name = updatedItem.Name;
    item.IsComplete = updatedItem.IsComplete;
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/tasks/{id}", async (ToDoDbContext db, int id) =>
{
    var item = await db.Tasks.FindAsync(id);
    if (item == null) return Results.NotFound();

    db.Tasks.Remove(item);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();