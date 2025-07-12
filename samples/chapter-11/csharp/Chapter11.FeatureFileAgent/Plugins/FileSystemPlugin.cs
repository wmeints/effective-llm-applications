using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace Chapter11.FeatureFileAgent.Plugins;

/// <summary>
/// Plugin for reading files from project documentation.
/// Operations are restricted to the specified documentation folder.
/// </summary>
public class FileSystemPlugin
{
    private readonly string _documentationFolder;

    public FileSystemPlugin(string documentationFolder)
    {
        if (string.IsNullOrWhiteSpace(documentationFolder))
        {
            throw new ArgumentException("Documentation folder path cannot be null or empty.", nameof(documentationFolder));
        }

        _documentationFolder = Path.GetFullPath(documentationFolder);

        if (!Directory.Exists(_documentationFolder))
        {
            throw new DirectoryNotFoundException($"Documentation folder does not exist: {_documentationFolder}");
        }
    }

    /// <summary>
    /// Lists all files in the documentation folder with their relative paths.
    /// </summary>
    /// <returns>A list of files with relative paths from the documentation folder root.</returns>
    [KernelFunction]
    [Description("List all files in the documentation folder with relative paths")]
    public Task<string> ListFilesAsync()
    {
        try
        {
            var files = Directory.GetFiles(_documentationFolder, "*", SearchOption.AllDirectories);

            if (!files.Any())
            {
                return Task.FromResult("No files found in the documentation folder.");
            }

            var result = $"Files in documentation folder ({_documentationFolder}):\n\n";

            foreach (var file in files.OrderBy(f => f))
            {
                var relativePath = Path.GetRelativePath(_documentationFolder, file);
                var fileInfo = new FileInfo(file);
                result += $"- {relativePath} ({fileInfo.Length} bytes, modified: {fileInfo.LastWriteTime:yyyy-MM-dd HH:mm})\n";
            }

            return Task.FromResult(result);
        }
        catch (Exception ex)
        {
            return Task.FromResult($"Error listing files: {ex.Message}");
        }
    }

    /// <summary>
    /// Reads the content of a file by its relative path.
    /// </summary>
    /// <param name="relativePath">The relative path to the file to read.</param>
    /// <returns>The content of the file.</returns>
    [KernelFunction]
    [Description("Read the content of a file by its relative path")]
    public async Task<string> ReadFileAsync(
        [Description("The relative path to the file to read")] string relativePath)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return "Relative path cannot be null or empty.";
            }

            // Ensure the path is safe and within the documentation folder
            var fullPath = Path.Combine(_documentationFolder, relativePath);
            var normalizedPath = Path.GetFullPath(fullPath);

            if (!normalizedPath.StartsWith(_documentationFolder, StringComparison.OrdinalIgnoreCase))
            {
                return "Access denied: Path is outside the documentation folder.";
            }

            if (!File.Exists(normalizedPath))
            {
                return $"File not found: {relativePath}";
            }

            var content = await File.ReadAllTextAsync(normalizedPath);

            if (string.IsNullOrWhiteSpace(content))
            {
                return $"File is empty: {relativePath}";
            }

            return content;
        }
        catch (Exception ex)
        {
            return $"Error reading file '{relativePath}': {ex.Message}";
        }
    }

    /// <summary>
    /// Searches for files containing the specified pattern in their content.
    /// </summary>
    /// <param name="searchPattern">The search pattern to find in file contents.</param>
    /// <returns>A list of files containing the search pattern with relevant snippets.</returns>
    [KernelFunction]
    [Description("Search for files containing the specified pattern in their content")]
    public async Task<string> FindFilesAsync(
        [Description("The search pattern to find in file contents")] string searchPattern)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchPattern))
            {
                return "Search pattern cannot be null or empty.";
            }

            var files = Directory.GetFiles(_documentationFolder, "*", SearchOption.AllDirectories);
            var matches = new List<FileMatch>();

            foreach (var file in files)
            {
                try
                {
                    // Only search text files (basic filter)
                    var extension = Path.GetExtension(file).ToLowerInvariant();
                    if (!IsTextFile(extension))
                    {
                        continue;
                    }

                    var content = await File.ReadAllTextAsync(file);

                    if (content.Contains(searchPattern, StringComparison.OrdinalIgnoreCase))
                    {
                        var relativePath = Path.GetRelativePath(_documentationFolder, file);
                        var snippets = ExtractSnippets(content, searchPattern);
                        matches.Add(new FileMatch { RelativePath = relativePath, Snippets = snippets });
                    }
                }
                catch (Exception)
                {
                    // Skip files that can't be read
                    continue;
                }
            }

            if (!matches.Any())
            {
                return $"No files found containing the pattern: {searchPattern}";
            }

            var result = $"Found {matches.Count} file(s) containing '{searchPattern}':\n\n";

            foreach (var match in matches.Take(10)) // Limit to first 10 matches
            {
                result += $"File: {match.RelativePath}\n";
                foreach (var snippet in match.Snippets.Take(3)) // Limit to 3 snippets per file
                {
                    result += $"  - ...{snippet}...\n";
                }
                result += "\n";
            }

            if (matches.Count > 10)
            {
                result += $"... and {matches.Count - 10} more files.\n";
            }

            return result;
        }
        catch (Exception ex)
        {
            return $"Error searching files: {ex.Message}";
        }
    }

    /// <summary>
    /// Gets information about the documentation folder.
    /// </summary>
    /// <returns>Information about the documentation folder.</returns>
    [KernelFunction]
    [Description("Get information about the documentation folder")]
    public Task<string> GetFolderInfoAsync()
    {
        try
        {
            var directoryInfo = new DirectoryInfo(_documentationFolder);
            var files = Directory.GetFiles(_documentationFolder, "*", SearchOption.AllDirectories);
            var directories = Directory.GetDirectories(_documentationFolder, "*", SearchOption.AllDirectories);

            var totalSize = files.Sum(file => new FileInfo(file).Length);

            var result = $"Documentation Folder: {_documentationFolder}\n" +
                   $"Exists: {directoryInfo.Exists}\n" +
                   $"Total Files: {files.Length}\n" +
                   $"Total Directories: {directories.Length}\n" +
                   $"Total Size: {FormatBytes(totalSize)}\n" +
                   $"Last Modified: {directoryInfo.LastWriteTime:yyyy-MM-dd HH:mm:ss}";

            return Task.FromResult(result);
        }
        catch (Exception ex)
        {
            return Task.FromResult($"Error getting folder info: {ex.Message}");
        }
    }

    private static bool IsTextFile(string extension)
    {
        var textExtensions = new[]
        {
            ".txt", ".md", ".markdown", ".rst", ".doc", ".docx", ".pdf",
            ".cs", ".js", ".ts", ".py", ".java", ".cpp", ".c", ".h",
            ".html", ".htm", ".xml", ".json", ".yaml", ".yml",
            ".cfg", ".config", ".ini", ".log", ".csv"
        };

        return textExtensions.Contains(extension);
    }

    private static List<string> ExtractSnippets(string content, string searchPattern)
    {
        var snippets = new List<string>();
        var lines = content.Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Contains(searchPattern, StringComparison.OrdinalIgnoreCase))
            {
                var startIndex = Math.Max(0, i - 1);
                var endIndex = Math.Min(lines.Length - 1, i + 1);

                var snippet = string.Join(" ", lines[startIndex..(endIndex + 1)])
                    .Trim()
                    .Replace('\t', ' ')
                    .Replace("  ", " ");

                if (snippet.Length > 200)
                {
                    snippet = snippet[..200];
                }

                snippets.Add(snippet);

                if (snippets.Count >= 5) // Limit snippets per file
                {
                    break;
                }
            }
        }

        return snippets;
    }

    private static string FormatBytes(long bytes)
    {
        const long KB = 1024;
        const long MB = KB * 1024;
        const long GB = MB * 1024;

        return bytes switch
        {
            >= GB => $"{bytes / (double)GB:F2} GB",
            >= MB => $"{bytes / (double)MB:F2} MB",
            >= KB => $"{bytes / (double)KB:F2} KB",
            _ => $"{bytes} bytes"
        };
    }
}

/// <summary>
/// Represents a file match with its relative path and content snippets.
/// </summary>
internal class FileMatch
{
    public string RelativePath { get; set; } = string.Empty;
    public List<string> Snippets { get; set; } = new();
}
