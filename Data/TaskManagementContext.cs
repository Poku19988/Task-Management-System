
using ConsoleApp.Data.Models;
using Microsoft.EntityFrameworkCore;
public class TaskManagementContext : DbContext
{
    public TaskManagementContext(DbContextOptions<TaskManagementContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<TaskHistory> TaskHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // تعریف رابطه یک به چند بین User و TaskItem (از سمت Creator)
    modelBuilder.Entity<TaskItem>()
        .HasOne(t => t.Creator)
        .WithMany()
        .HasForeignKey(t => t.CreatorId)
        .OnDelete(DeleteBehavior.Restrict);

    // تعریف رابطه یک به چند بین User و TaskItem (از سمت Assignee)
    modelBuilder.Entity<TaskItem>()
        .HasOne(t => t.Assignee)
        .WithMany(u => u.Tasks)
        .HasForeignKey(t => t.AssigneeId)
        .OnDelete(DeleteBehavior.SetNull);

    // تعریف رابطه یک به چند بین TaskItem و Comment
    modelBuilder.Entity<Comment>()
        .HasOne(c => c.TaskItem)
        .WithMany(t => t.Comments)
        .HasForeignKey(c => c.TaskId)
        .OnDelete(DeleteBehavior.Cascade);

    // تعریف رابطه یک به چند بین User و Comment
    modelBuilder.Entity<Comment>()
        .HasOne(c => c.User)
        .WithMany(u => u.Comments)
        .HasForeignKey(c => c.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    // تعریف رابطه یک به چند بین TaskItem و TaskHistory
    modelBuilder.Entity<TaskHistory>()
        .HasOne(th => th.TaskItem)
        .WithMany(t => t.TaskHistories)
        .HasForeignKey(th => th.TaskId)
        .OnDelete(DeleteBehavior.Cascade);

    // مقداردهی اولیه برای User (نمونه ادمین)
    modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Username = "admin", Password = "admin", Role = "Admin" },
            new User { Id = 2, Username = "user1", Password = "user1password", Role = "Regular" }
        );
}
}

