using ConsoleApp.Data.Enums;

namespace ConsoleApp.Data.Models;

public class TaskHistory
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public TaskItem TaskItem { get; set; } = null!;
    public TimeSpan TimeSpent { get; set; }
    public string Description { get; set; } = null!;
    public TaskItemStatus Status { get; set; }
    public DateTime ChangedAt { get; set; }

    public static TaskHistory LogHistory(TaskItem task)
    {
        return new TaskHistory
        {
            TaskId = task.Id,
            TaskItem = task,
            TimeSpent = task.TimeSpent,
            Description = task.Description,
            Status = task.Status,
            ChangedAt = DateTime.Now
        };
    }
}
