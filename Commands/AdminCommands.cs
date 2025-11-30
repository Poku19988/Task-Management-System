using ConsoleApp.Data;
using ConsoleApp.Data.Models;
using ConsoleApp.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp.Commands;

public class AdminCommands
{
    private readonly TaskManagementContext _context;

    public AdminCommands(TaskManagementContext context)
    {
        _context = context;
    }

    public async Task CreateUserAsync()
    {
        var username = Console.ReadLine();
        var password = Console.ReadLine();
        var role = Console.ReadLine();

        if (string.IsNullOrEmpty(username))
        {
            Console.WriteLine("Username cannot be empty.");
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            Console.WriteLine("PassWord cannot be empty.");
            return;
        }

        if (role != "Admin" && role != "Regular")
        {
            Console.WriteLine("Invalid role.");
            return;
        }

        var user = new User() { Username = username, Password = password, Role = role };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        Console.WriteLine("User created successfully.");
    }

    public async Task ListUsersAsync()
    {
        var users = await _context.Users.ToListAsync();
        foreach (var user in users)
        {
            Console.WriteLine($"ID: {user.Id}, Username: {user.Username}, Role: {user.Role}");
        }
    }

    public async Task DeleteUserAsync()
    {
        var input = Console.ReadLine();
        if (!int.TryParse(input, out int id))
        {
            Console.WriteLine("Invalid input.");
            return;
        }

        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            Console.WriteLine("User not found.");
            return;
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        Console.WriteLine("User deleted successfully.");
    }

    public async Task CreateTaskAsync()
    {
        var description = Console.ReadLine();
        if (string.IsNullOrEmpty(description))
        {
            Console.WriteLine("Description cannot be empty.");
            return;
        }

        var creatorIdInput = Console.ReadLine();
        if (!int.TryParse(creatorIdInput, out int creatorId))
        {
            Console.WriteLine("Invalid creator ID.");
            return;
        }

        var creator = await _context.Users.FindAsync(creatorId);
        if (creator == null)
        {
            Console.WriteLine("Creator ID does not exist.");
            return;
        }

        var assigneeIdInput = Console.ReadLine();
        int? assigneeId = null;
        if (!string.IsNullOrEmpty(assigneeIdInput))
        {
            if (!int.TryParse(assigneeIdInput, out int tempId))
            {
                Console.WriteLine("Invalid assignee ID.");
                return;
            }
            
            var assignee = await _context.Users.FindAsync(tempId);
            if (assignee == null)
            {
                Console.WriteLine("Assignee ID does not exist.");
                return;
            }
            assigneeId = tempId;
        }

        var task = new TaskItem
        {
            Description = description,
            CreatorId = creatorId,
            AssigneeId = assigneeId,
            Status = TaskItemStatus.ToDo,
            TimeSpent = TimeSpan.Zero
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        
        // Log history
        _context.TaskHistories.Add(TaskHistory.LogHistory(task));
        await _context.SaveChangesAsync();
        
        Console.WriteLine("Task created successfully.");
    }

    public async Task ListTasksAsync()
{
    var tasks = await _context.Tasks
        .Include(t => t.Assignee)  
        .Include(t => t.Creator)
        .ToListAsync();

    foreach (var task in tasks) 
    {
        string assigneeName = task.Assignee?.Username ?? task.Creator.Username ?? "None";  

        Console.WriteLine($"{task.Id}: {task.Description}, Status: {task.Status}, Assignee: {assigneeName}");
    }
}


    public async Task PromoteUserToAdminAsync()
{
    var input = Console.ReadLine();
    if (!int.TryParse(input, out int userId))
    {
        Console.WriteLine("Invalid input.");
        return;
    }

    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
    if (user == null)
    {
        Console.WriteLine("User not found.");
        return;
    }

    if (user.Role == "Admin")
    {
        Console.WriteLine("User is already an Admin.");
        return;
    }

    if (user.Role != "Regular")
    {
        Console.WriteLine("User has an invalid role. Promotion cannot proceed.");
        return;
    }

    user.Role = "Admin";
    await _context.SaveChangesAsync();
    Console.WriteLine("User promoted to Admin successfully.");
}
}
