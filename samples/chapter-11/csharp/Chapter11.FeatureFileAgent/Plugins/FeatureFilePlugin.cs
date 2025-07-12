using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace Chapter11.FeatureFileAgent.Plugins;

/// <summary>
/// Plugin for managing feature file operations.
/// Only works with the specified feature file provided during initialization.
/// </summary>
public class FeatureFilePlugin
{
    private readonly string _featureFilePath;
    private readonly object _lock = new object();

    public FeatureFilePlugin(string featureFilePath)
    {
        if (string.IsNullOrWhiteSpace(featureFilePath))
        {
            throw new ArgumentException("Feature file path cannot be null or empty.", nameof(featureFilePath));
        }

        _featureFilePath = Path.GetFullPath(featureFilePath);

        // Create the directory if it doesn't exist
        var directory = Path.GetDirectoryName(_featureFilePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Create the file if it doesn't exist
        if (!File.Exists(_featureFilePath))
        {
            File.WriteAllText(_featureFilePath, string.Empty);
        }
    }

    // /// <summary>
    // /// Inserts content into the feature file at the specified line number.
    // /// </summary>
    // /// <param name="content">The content to insert.</param>
    // /// <param name="lineNumber">The line number to insert at (1-based).</param>
    // /// <returns>A confirmation message.</returns>
    // [KernelFunction]
    // [Description("Insert content into the feature file at a specific line number")]
    // public async Task<string> InsertContentAtLineAsync(
    //     [Description("The content to insert")] string content,
    //     [Description("The line number to insert at (1-based)")] int lineNumber)
    // {
    //     try
    //     {
    //         var lines = File.Exists(_featureFilePath) ?
    //             (await File.ReadAllLinesAsync(_featureFilePath)).ToList() :
    //             new List<string>();

    //         if (lineNumber < 1)
    //         {
    //             return "Line number must be 1 or greater.";
    //         }

    //         // Ensure we have enough lines
    //         while (lines.Count < lineNumber - 1)
    //         {
    //             lines.Add(string.Empty);
    //         }

    //         // Insert the content at the specified line (convert to 0-based index)
    //         lines.Insert(lineNumber - 1, content);

    //         await File.WriteAllLinesAsync(_featureFilePath, lines);
    //         return $"Content inserted successfully at line {lineNumber}.";
    //     }
    //     catch (Exception ex)
    //     {
    //         return $"Error inserting content: {ex.Message}";
    //     }
    // }

    // /// <summary>
    // /// Appends content to the end of the feature file.
    // /// </summary>
    // /// <param name="content">The content to append.</param>
    // /// <returns>A confirmation message.</returns>
    // [KernelFunction]
    // [Description("Append content to the end of the feature file")]
    // public async Task<string> AppendContentAsync(
    //     [Description("The content to append")] string content)
    // {
    //     try
    //     {
    //         await File.AppendAllTextAsync(_featureFilePath, content + Environment.NewLine);
    //         return "Content appended successfully.";
    //     }
    //     catch (Exception ex)
    //     {
    //         return $"Error appending content: {ex.Message}";
    //     }
    // }

    // /// <summary>
    // /// Replaces existing content in the feature file with new content.
    // /// </summary>
    // /// <param name="originalContent">The original content to replace.</param>
    // /// <param name="newContent">The new content to replace it with.</param>
    // /// <returns>A confirmation message.</returns>
    // [KernelFunction]
    // [Description("Replace existing content in the feature file with new content")]
    // public async Task<string> ReplaceContentAsync(
    //     [Description("The original content to replace")] string originalContent,
    //     [Description("The new content to replace it with")] string newContent)
    // {
    //     try
    //     {
    //         if (!File.Exists(_featureFilePath))
    //         {
    //             return "Feature file does not exist.";
    //         }

    //         var content = await File.ReadAllTextAsync(_featureFilePath);

    //         if (!content.Contains(originalContent))
    //         {
    //             return "Original content not found in the feature file.";
    //         }

    //         var newFileContent = content.Replace(originalContent, newContent);
    //         await File.WriteAllTextAsync(_featureFilePath, newFileContent);

    //         return "Content replaced successfully.";
    //     }
    //     catch (Exception ex)
    //     {
    //         return $"Error replacing content: {ex.Message}";
    //     }
    // }

    // /// <summary>
    // /// Removes specific content from the feature file.
    // /// </summary>
    // /// <param name="contentToRemove">The content to remove.</param>
    // /// <returns>A confirmation message.</returns>
    // [KernelFunction]
    // [Description("Remove specific content from the feature file")]
    // public async Task<string> RemoveContentAsync(
    //     [Description("The content to remove")] string contentToRemove)
    // {
    //     try
    //     {
    //         if (!File.Exists(_featureFilePath))
    //         {
    //             return "Feature file does not exist.";
    //         }

    //         var content = await File.ReadAllTextAsync(_featureFilePath);

    //         if (!content.Contains(contentToRemove))
    //         {
    //             return "Content to remove not found in the feature file.";
    //         }

    //         var newContent = content.Replace(contentToRemove, string.Empty);
    //         await File.WriteAllTextAsync(_featureFilePath, newContent);

    //         return "Content removed successfully.";
    //     }
    //     catch (Exception ex)
    //     {
    //         return $"Error removing content: {ex.Message}";
    //     }
    // }

    /// <summary>
    /// Writes content to the feature file, overwriting any existing content.
    /// </summary>
    /// <param name="content">The content to write to the file.</param>
    /// <returns>A confirmation message.</returns>
    [KernelFunction]
    [Description("Write content to the feature file, overwriting any existing content")]
    public async Task<string> WriteContentAsync(
        [Description("The content to write to the file")] string content)
    {
        try
        {
            await File.WriteAllTextAsync(_featureFilePath, content);
            return "Content written successfully to the feature file.";
        }
        catch (Exception ex)
        {
            return $"Error writing content: {ex.Message}";
        }
    }

    /// <summary>
    /// Reads the current content of the feature file.
    /// </summary>
    /// <returns>The current content of the feature file.</returns>
    [KernelFunction]
    [Description("Read the current content of the feature file")]
    public async Task<string> ReadFeatureFileAsync()
    {
        try
        {
            if (!File.Exists(_featureFilePath))
            {
                return "Feature file does not exist or is empty.";
            }

            var content = await File.ReadAllTextAsync(_featureFilePath);
            return string.IsNullOrWhiteSpace(content) ? "Feature file is empty." : content;
        }
        catch (Exception ex)
        {
            return $"Error reading feature file: {ex.Message}";
        }
    }
}
