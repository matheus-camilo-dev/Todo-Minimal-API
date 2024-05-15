namespace Todo.Api;

internal sealed record TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public TodoItemStatus Status { get; set; }
}