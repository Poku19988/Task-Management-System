using ConsoleApp.Data;
using ConsoleApp.Data.Models;
using ConsoleApp.Data.Enums;

using Microsoft.EntityFrameworkCore;

namespace ConsoleApp.Commands;

public class RegularUserCommands
{
    private readonly TaskManagementContext _context;
    private readonly User _currentUser;

    public RegularUserCommands(TaskManagementContext context, User currentUser)
    {
        _context = context;
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    }

    public async Task ListMyTasksAsync()
    {
        var tasks = await _context.Tasks
            .Where(t => t.AssigneeId == _currentUser.Id)
            .ToListAsync();

        foreach (var task in tasks)
        {
            Console.WriteLine($"{task.Id}: {task.Description}, Status: {task.Status}");
            await PrintTaskComments(task.Id);
        }
    }

    public  Task UpdateTaskStatusAsync()
    {
        var taskIdInput = Console.ReadLine();
        if (!int.TryParse(taskIdInput, out int taskId))
        {
            Console.WriteLine("Invalid input.");
            return Task.CompletedTask;
        }

        var task = _context.Tasks.FirstOrDefault(x => x.Id == taskId);
        if (task == null || task.AssigneeId != _currentUser.Id)
        {
            Console.WriteLine("Task not found or you do not have permission to update this task.");
            return Task.CompletedTask;
        }

        var statusInput = Console.ReadLine();
        if (!Enum.TryParse<TaskItemStatus>(statusInput, out var newStatus))
        {
            Console.WriteLine("Invalid status.");
            return Task.CompletedTask;
        }

        // Log history before changing status
        
        
        task.Status = newStatus;
        _context.TaskHistories.Add(TaskHistory.LogHistory(task));
        Console.WriteLine("Task status updated successfully.");
        return Task.CompletedTask;
        
        
    }

    public async Task CreateTaskAsync()
    {
        var description = Console.ReadLine();
        var assigneeIdInput = Console.ReadLine();

        if (string.IsNullOrEmpty(description))
        {
            Console.WriteLine("Description cannot be empty.");
            return;
        }

        if (!int.TryParse(assigneeIdInput, out int assigneeId))
        {
            Console.WriteLine("Invalid assignee ID.");
            return;
        }

        var assignee = await _context.Users.FindAsync(assigneeId);
        if (assignee == null)
        {
            Console.WriteLine("Assignee ID does not exist.");
            return;
        }

        var task = new TaskItem
        {
            Description = description,
            CreatorId = _currentUser.Id,
            AssigneeId = assigneeId,
            Status = TaskItemStatus.ToDo,
            TimeSpent = TimeSpan.Zero
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        
        // Log history
        _context.TaskHistories.Add(TaskHistory.LogHistory(task));
        Console.WriteLine("Task created and assigned successfully.");
        await _context.SaveChangesAsync();
        
        
    }

    public async Task AddCommentAsync()
    {
        var taskIdInput = Console.ReadLine();
        if (!int.TryParse(taskIdInput, out int taskId))
        {
            Console.WriteLine("Invalid task ID.");
            return;
        }

        var task = await _context.Tasks.FindAsync(taskId);
        if (task == null)
        {
            Console.WriteLine("Task not found.");
            return;
        }

        var commentText = Console.ReadLine();
        if (string.IsNullOrEmpty(commentText))
        {
            Console.WriteLine("Comment text cannot be empty.");
            return;
        }

        var comment = new Comment
        {
            Text = commentText,
            UserId = _currentUser.Id,
            TaskId = taskId,
            PostedAt = DateTime.Now
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
        Console.WriteLine("Comment added successfully.");
    }

    public async Task UpdateTimeSpentAsync()
    {
        var taskIdInput = Console.ReadLine();
        if (!int.TryParse(taskIdInput, out int taskId))
        {
            Console.WriteLine("Invalid task ID.");
            return;
        }

        var task = await _context.Tasks.FindAsync(taskId);
        if (task == null || task.AssigneeId != _currentUser.Id)
        {
            Console.WriteLine("Task not found or you do not have permission to update this task.");
            return;
        }

        var timeInput = Console.ReadLine();
        if (!double.TryParse(timeInput, out double hours))
        {
            Console.WriteLine("Invalid time format.");
            return;
        }

        // Log history before changing time spent
        
        
        task.TimeSpent = TimeSpan.FromHours(hours);
        _context.TaskHistories.Add(TaskHistory.LogHistory(task));
        await _context.SaveChangesAsync();
        
        Console.WriteLine("Task time spent updated successfully.");
    }

    private async Task PrintTaskComments(int taskId)
    {
        var comments = await _context.Comments
            .Include(c => c.User)
            .Where(c => c.TaskId == taskId)
            .OrderBy(c => c.PostedAt)
            .ToListAsync();

        foreach (var comment in comments)
        {
            Console.WriteLine($"- [{comment.PostedAt}] {comment.User?.Username ?? "Unknown"}: {comment.Text}");
        }
    }
}