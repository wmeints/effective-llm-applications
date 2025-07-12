using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json;

namespace Chapter11.FeatureFileAgent.Plugins;

/// <summary>
/// Plugin for managing TODO items to help the agent track its planning tasks.
/// TODO items are persisted in a JSON file within the .agent folder.
/// </summary>
public class TodoItemsPlugin
{
    private readonly string _todoFilePath;
    private readonly object _lock = new object();

    public TodoItemsPlugin()
    {
        var agentFolder = Path.Combine(Directory.GetCurrentDirectory(), ".agent");
        Directory.CreateDirectory(agentFolder);
        _todoFilePath = Path.Combine(agentFolder, "todo-items.json");
    }

    /// <summary>
    /// Creates a new TODO item with the specified title.
    /// </summary>
    /// <param name="title">The title of the TODO item to create.</param>
    /// <returns>A confirmation message.</returns>
    [KernelFunction]
    [Description("Create a new TODO item with the specified title")]
    public Task<string> CreateTodoItemAsync(
        [Description("The title of the TODO item to create")] string title)
    {
        lock (_lock)
        {
            var todoItems = LoadTodoItems();

            if (todoItems.Any(item => item.Title.Equals(title, StringComparison.OrdinalIgnoreCase)))
            {
                return Task.FromResult($"TODO item '{title}' already exists.");
            }

            todoItems.Add(new TodoItem
            {
                Title = title,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            });

            SaveTodoItems(todoItems);
            return Task.FromResult($"TODO item '{title}' created successfully.");
        }
    }

    /// <summary>
    /// Marks a TODO item as completed by its title.
    /// </summary>
    /// <param name="title">The title of the TODO item to complete.</param>
    /// <returns>A confirmation message.</returns>
    [KernelFunction]
    [Description("Mark a TODO item as completed by its title")]
    public Task<string> CompleteTodoItemAsync(
        [Description("The title of the TODO item to complete")] string title)
    {
        lock (_lock)
        {
            var todoItems = LoadTodoItems();
            var item = todoItems.FirstOrDefault(item => item.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

            if (item == null)
            {
                return Task.FromResult($"TODO item '{title}' not found.");
            }

            if (item.IsCompleted)
            {
                return Task.FromResult($"TODO item '{title}' is already completed.");
            }

            item.IsCompleted = true;
            item.CompletedAt = DateTime.UtcNow;

            SaveTodoItems(todoItems);
            return Task.FromResult($"TODO item '{title}' marked as completed.");
        }
    }

    /// <summary>
    /// Removes a TODO item by its title.
    /// </summary>
    /// <param name="title">The title of the TODO item to remove.</param>
    /// <returns>A confirmation message.</returns>
    [KernelFunction]
    [Description("Remove a TODO item by its title")]
    public Task<string> RemoveTodoItemAsync(
        [Description("The title of the TODO item to remove")] string title)
    {
        lock (_lock)
        {
            var todoItems = LoadTodoItems();
            var item = todoItems.FirstOrDefault(item => item.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

            if (item == null)
            {
                return Task.FromResult($"TODO item '{title}' not found.");
            }

            todoItems.Remove(item);
            SaveTodoItems(todoItems);
            return Task.FromResult($"TODO item '{title}' removed successfully.");
        }
    }

    /// <summary>
    /// Lists all current TODO items.
    /// </summary>
    /// <returns>A formatted list of all TODO items.</returns>
    [KernelFunction]
    [Description("List all current TODO items")]
    public Task<string> ListTodoItemsAsync()
    {
        lock (_lock)
        {
            var todoItems = LoadTodoItems();

            if (!todoItems.Any())
            {
                return Task.FromResult("No TODO items found.");
            }

            var result = "Current TODO items:\n";
            var pendingItems = todoItems.Where(item => !item.IsCompleted).ToList();
            var completedItems = todoItems.Where(item => item.IsCompleted).ToList();

            if (pendingItems.Any())
            {
                result += "\nPending:\n";
                foreach (var item in pendingItems)
                {
                    result += $"- {item.Title} (Created: {item.CreatedAt:yyyy-MM-dd HH:mm})\n";
                }
            }

            if (completedItems.Any())
            {
                result += "\nCompleted:\n";
                foreach (var item in completedItems)
                {
                    result += $"- {item.Title} (Completed: {item.CompletedAt:yyyy-MM-dd HH:mm})\n";
                }
            }

            return Task.FromResult(result);
        }
    }

    private List<TodoItem> LoadTodoItems()
    {
        if (!File.Exists(_todoFilePath))
        {
            return new List<TodoItem>();
        }

        try
        {
            var json = File.ReadAllText(_todoFilePath);
            return JsonSerializer.Deserialize<List<TodoItem>>(json) ?? new List<TodoItem>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading TODO items: {ex.Message}");
            return new List<TodoItem>();
        }
    }

    private void SaveTodoItems(List<TodoItem> todoItems)
    {
        try
        {
            var json = JsonSerializer.Serialize(todoItems, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(_todoFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving TODO items: {ex.Message}");
        }
    }
}

/// <summary>
/// Represents a TODO item with title, completion status, and timestamps.
/// </summary>
public class TodoItem
{
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
