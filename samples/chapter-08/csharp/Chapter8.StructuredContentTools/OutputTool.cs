using System.ComponentModel;
using Microsoft.SemanticKernel;

public class OutputTool
{
    public string Title { get; set; } = string.Empty;
    public List<string> Steps { get; set; } = new();

    [KernelFunction, Description("Store the created user story")]
    public void CreateUserStory(string title, List<string> steps)
    {
        Title = title;
        Steps = steps;
    }
}