# Task-Management-System




A simple task management system designed for a recruitment assignment.
The system includes user management, task management, access control (Admin / RegularUser), comments, time logging, and task history tracking.

---

## Features

### User Management

* Create, edit, and delete users.
* Two user roles:

  * **Admin** – full access to all users and all tasks.
  * **Regular User** – limited access.
* Admins can:

  * Add new users
  * Remove users
  * Change user roles

### Task Management

* Users can create tasks and assign them to others.
* Each task contains:

  * **Description** — detailed information about the task
  * **Creator** — the user who created the task
  * **Assignee** — the user assigned to complete it
  * **Comments** — notes and feedback
  * **TimeSpent** — time logged on the task
  * **Status** — e.g. `Pending`, `InProgress`, `InTesting`, `Done`
* Admins can view and edit all tasks.
* Regular users can only view/edit tasks assigned to them.

### Task History

* Whenever a task is updated, a previous version is stored in the **TaskHistory** table.
* History entries include:

  * Description
  * Status
  * Time spent
  * Timestamp of change

---

## Files to Complete

* **AdminCommands.cs** – admin-only operations
* **RegularUserCommands.cs** – regular user operations
* **TaskItemStatus.cs** – enum for task statuses
* **TaskManagementContext.cs** – EF Core context and model configuration

### Model Files

#### TaskCommentModel.cs

| Name     | Type     | Description         |
| -------- | -------- | ------------------- |
| Id       | int      | Primary key         |
| UserId   | int      | Foreign key         |
| User     | User     | Navigation property |
| TaskId   | int      | Foreign key         |
| TaskItem | TaskItem | Navigation property |
| Text     | string   | Comment text        |
| PostedAt | DateTime | Timestamp           |

#### TaskHistory.cs

| Name                      | Type           | Description                                         |
| ------------------------- | -------------- | --------------------------------------------------- |
| Id                        | int            | Primary key                                         |
| TaskId                    | int            | Foreign key                                         |
| TaskItem                  | TaskItem       | Navigation property                                 |
| TimeSpent                 | TimeSpan       | Logged time at that version                         |
| Description               | string         | Description at that moment                          |
| Status                    | TaskItemStatus | Status at that moment                               |
| ChangedAt                 | DateTime       | Timestamp of change                                 |
| LogHistory(TaskItem task) | static         | Creates a history entry from the current task state |

#### TaskModel.cs

| Name          | Type                     | Description            |
| ------------- | ------------------------ | ---------------------- |
| Id            | int                      | Primary key            |
| Description   | string                   | Task description       |
| CreatorId     | int                      | Foreign key            |
| Creator       | User                     | Navigation property    |
| AssigneeId    | int?                     | Foreign key (optional) |
| Assignee      | User?                    | Navigation property    |
| Comments      | List<TaskCommentModel>   | Attached comments      |
| TimeSpent     | TimeSpan                 | Total logged time      |
| Status        | TaskItemStatus           | Current status         |
| TaskHistories | ICollection<TaskHistory> | All history entries    |

---

## Access Rules

### Admin

* Full control over users
* Full control over all tasks
* Full access to task histories

### Regular User

* Create new tasks
* Edit/view only tasks assigned to them
* Add comments
* Log time

---

## Technology

* **C#**, **.NET 6+**
* **Entity Framework Core**
* Any EF-supported relational database (SQL Server, PostgreSQL, SQLite, etc.)

---

## Setup

```bash
dotnet restore
dotnet build
dotnet ef database update
dotnet run
```




Just tell me!
