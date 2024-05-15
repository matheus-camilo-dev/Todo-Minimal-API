using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Todo.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

#region GET
app.MapGet("/todoitems", async (TodoDb db) =>
    await db.TodoItems.ToListAsync())
        .Produces<IList<TodoItem>>();

app.MapGet("/todoitems/status", (TodoDb db) =>
    Enum.GetValues(typeof(TodoItemStatus))
        .Cast<TodoItemStatus>()
        .ToDictionary(t => t.ToString(), t => (int)t))
    .Produces<IEnumerable<TodoItemStatus>>();

app.MapGet("/todoitems/status/{todoItemStatus}", async (TodoItemStatus todoItemStatus, TodoDb db) =>
    await db.TodoItems.Where(t => t.Status == todoItemStatus).ToListAsync())
        .Produces<IList<TodoItem>>();

app.MapGet("/todoitems/{id}", async (int id, TodoDb db) =>
    await db.TodoItems.FindAsync(id)
        is TodoItem todoItem
            ? Results.Ok(todoItem)
            : Results.NotFound())
        .Produces<TodoItem>()
        .Produces(StatusCodes.Status404NotFound);

#endregion

#region POST
app.MapPost("/todoitems", async (TodoItem todoItem, TodoDb db) =>
{
    db.TodoItems.Add(todoItem);
    await db.SaveChangesAsync();

    return Results.Created($"/todoitems/{todoItem.Id}", todoItem);
}).Produces<TodoItem>();
#endregion

#region PUT
app.MapPut("/todoitems/{id}", async (int id, TodoItem updatedTodoItem, TodoDb db) =>
{
    var todoItem = await db.TodoItems.FindAsync(id);

    if (todoItem is null) return Results.NotFound();

    todoItem.Title = updatedTodoItem.Title;
    todoItem.Status = updatedTodoItem.Status;

    await db.SaveChangesAsync();

    return Results.NoContent();
}).Produces<NoContentResult>()
  .Produces(StatusCodes.Status404NotFound);
#endregion

#region DELETE
app.MapDelete("/todoitems/{id}", async (int id, TodoDb db) =>
{
    if (await db.TodoItems.FindAsync(id) is TodoItem todo)
    {
        db.TodoItems.Remove(todo);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
}).Produces<NoContentResult>()
  .Produces(StatusCodes.Status404NotFound);
#endregion

app.Run();