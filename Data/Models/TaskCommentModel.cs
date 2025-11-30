namespace ConsoleApp.Data.Models;

public class Comment
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;  // اضافه شد

    public int TaskId { get; set; }
    public TaskItem TaskItem { get; set; } = null!;

    public string Text { get; set; } = null!;
    public DateTime PostedAt { get; set; }
}
