using Microsoft.EntityFrameworkCore;
using Placeholder.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var api = app.MapGroup("/api/v1");

api.MapGet("/posts", async (ApplicationDbContext db) => await db.Posts.ToListAsync());
api.MapGet("/posts/{id:int}", async (int id, ApplicationDbContext db) => await db.Posts.FindAsync(id) is var p && p != null ? Results.Ok(p) : Results.NotFound());
api.MapPost("/posts", async (Post newPost, ApplicationDbContext db) => { db.Posts.Add(newPost); await db.SaveChangesAsync(); return Results.Created($"/api/v1/posts/{newPost.Id}", newPost); });
api.MapPut("/posts/{id:int}", async (int id, Post updatedPost, ApplicationDbContext db) => { var e = await db.Posts.FindAsync(id); if (e == null) return Results.NotFound(); e.UserId = updatedPost.UserId; e.Title = updatedPost.Title; e.Body = updatedPost.Body; await db.SaveChangesAsync(); return Results.NoContent(); });
api.MapDelete("/posts/{id:int}", async (int id, ApplicationDbContext db) => { var e = await db.Posts.FindAsync(id); if (e == null) return Results.NotFound(); db.Posts.Remove(e); await db.SaveChangesAsync(); return Results.NoContent(); });

api.MapGet("/comments", async (ApplicationDbContext db) => await db.Comments.ToListAsync());
api.MapGet("/comments/{id:int}", async (int id, ApplicationDbContext db) => await db.Comments.FindAsync(id) is var c && c != null ? Results.Ok(c) : Results.NotFound());
api.MapPost("/comments", async (Comment newComment, ApplicationDbContext db) => { db.Comments.Add(newComment); await db.SaveChangesAsync(); return Results.Created($"/api/v1/comments/{newComment.Id}", newComment); });
api.MapPut("/comments/{id:int}", async (int id, Comment updatedComment, ApplicationDbContext db) => { var e = await db.Comments.FindAsync(id); if (e == null) return Results.NotFound(); e.PostId = updatedComment.PostId; e.Name = updatedComment.Name; e.Email = updatedComment.Email; e.Body = updatedComment.Body; await db.SaveChangesAsync(); return Results.NoContent(); });
api.MapDelete("/comments/{id:int}", async (int id, ApplicationDbContext db) => { var e = await db.Comments.FindAsync(id); if (e == null) return Results.NotFound(); db.Comments.Remove(e); await db.SaveChangesAsync(); return Results.NoContent(); });

api.MapGet("/users", async (ApplicationDbContext db) => await db.Users.ToListAsync());
api.MapGet("/users/{id:int}", async (int id, ApplicationDbContext db) => await db.Users.FindAsync(id) is var u && u != null ? Results.Ok(u) : Results.NotFound());
api.MapPost("/users", async (User newUser, ApplicationDbContext db) => { db.Users.Add(newUser); await db.SaveChangesAsync(); return Results.Created($"/api/v1/users/{newUser.Id}", newUser); });
api.MapPut("/users/{id:int}", async (int id, User updatedUser, ApplicationDbContext db) => { var e = await db.Users.FindAsync(id); if (e == null) return Results.NotFound(); e.Name = updatedUser.Name; e.Username = updatedUser.Username; e.Email = updatedUser.Email; await db.SaveChangesAsync(); return Results.NoContent(); });
api.MapDelete("/users/{id:int}", async (int id, ApplicationDbContext db) => { var e = await db.Users.FindAsync(id); if (e == null) return Results.NotFound(); db.Users.Remove(e); await db.SaveChangesAsync(); return Results.NoContent(); });

app.Run();
