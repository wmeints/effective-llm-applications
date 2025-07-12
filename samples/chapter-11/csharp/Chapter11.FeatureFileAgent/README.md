# Feature File Generator Agent

This project demonstrates the implementation of an intelligent agent capable of generating feature files using Semantic Kernel. The agent serves as a practical example for readers of "Building effective LLM-based applications with Semantic Kernel" to understand agent creation, tool development, and plugin integration.

## Project Structure

```
Chapter11.FeatureFileAgent/
├── Plugins/
│   ├── TodoItemsPlugin.cs      # Manages TODO items for agent planning
│   ├── FeatureFilePlugin.cs    # Handles feature file operations
│   └── FileSystemPlugin.cs     # Provides documentation file access
├── Program.cs                  # Main agent application (requires AI service)
├── TestProgram.cs              # Test program demonstrating plugin functionality
└── README.md                   # This file
```

## Features

### TodoItemsPlugin
- **Create TODO items**: Help the agent track planning tasks
- **Complete TODO items**: Mark tasks as finished
- **Remove TODO items**: Clean up completed or unnecessary items
- **List TODO items**: View current and completed tasks
- **Persistence**: TODO items are saved to `.agent/todo-items.json`

### FeatureFilePlugin
- **Insert content**: Add content at specific line numbers
- **Append content**: Add content to the end of the file
- **Replace content**: Update existing content with new content
- **Remove content**: Delete specific content from the file
- **Read file**: View current feature file content
- **File info**: Get metadata about the feature file

### FileSystemPlugin
- **List files**: Show all files in the documentation folder
- **Read files**: Access content of specific documentation files
- **Search files**: Find files containing specific patterns
- **Folder info**: Get information about the documentation directory

## Getting Started

### Prerequisites
- .NET 8.0 or later
- Access to an AI service (Azure OpenAI, OpenAI, etc.) for the full agent experience

### Building the Project
```bash
dotnet build
```

### Running the Test Program
To test the plugins without requiring AI services:

```bash
dotnet run "test.feature" "./"
```

This will:
1. Create TODO items demonstrating planning capabilities
2. Generate a sample feature file with Gherkin syntax
3. List and analyze documentation files in the current directory
4. Show persistence of TODO items across runs

### Running the Full Agent
To run the full agent with AI capabilities:

1. Configure your AI service credentials (see Configuration section)
2. Update the `StartupObject` in the project file to use `Program` instead of `TestProgram`
3. Run the application:

```bash
dotnet run "path/to/your/feature.feature" "path/to/your/docs"
```

## Configuration

### AI Service Setup
The full agent requires configuration of an AI service. You can use either Azure OpenAI or OpenAI.

#### For Azure OpenAI:
Add these to your user secrets or environment variables:
```json
{
  "AzureOpenAI:Endpoint": "https://your-endpoint.openai.azure.com/",
  "AzureOpenAI:ApiKey": "your-api-key",
  "AzureOpenAI:DeploymentName": "your-deployment-name"
}
```

#### For OpenAI:
```json
{
  "OpenAI:ApiKey": "your-openai-api-key"
}
```

### Setting User Secrets
```bash
dotnet user-secrets set "AzureOpenAI:ApiKey" "your-api-key"
dotnet user-secrets set "AzureOpenAI:Endpoint" "https://your-endpoint.openai.azure.com/"
dotnet user-secrets set "AzureOpenAI:DeploymentName" "your-deployment-name"
```

## Usage Examples

### Basic Feature File Generation
```bash
# Generate a feature file for user authentication
dotnet run "features/auth.feature" "docs/"
```

### Working with Existing Documentation
Place your project documentation in a folder and reference it:
```bash
dotnet run "features/payment.feature" "project-docs/"
```

The agent will:
1. Analyze your documentation
2. Create a plan using TODO items
3. Generate comprehensive feature files with proper Gherkin syntax
4. Include relevant scenarios based on your requirements

## Agent Capabilities

The agent is designed to:

### Planning and Organization
- Break down complex feature file creation into manageable tasks
- Track progress using TODO items
- Maintain context across multiple interactions

### Documentation Analysis
- Read and understand project documentation
- Extract requirements and business rules
- Search for specific information across multiple files

### Feature File Creation
- Generate well-structured Gherkin feature files
- Include proper Given-When-Then scenarios
- Follow best practices for behavior-driven development
- Support various Gherkin keywords and structures

### Interactive Assistance
- Respond to natural language requests
- Provide help and guidance
- Explain decisions and reasoning

## Example Output

### Sample Feature File Generated
```gherkin
Feature: User Authentication
  As a user
  I want to be able to log in
  So that I can access my account

  Scenario: Successful login
    Given I am on the login page
    When I enter valid credentials
    Then I should be logged in

  Scenario: Invalid credentials
    Given I am on the login page
    When I enter invalid credentials
    Then I should see an error message
```

### Sample TODO Items
```
Current TODO items:

Pending:
- Analyze requirements (Created: 2025-07-12 10:30)
- Add test scenarios (Created: 2025-07-12 10:30)

Completed:
- Create feature file structure (Completed: 2025-07-12 10:35)
```

## Architecture

The project demonstrates several key Semantic Kernel concepts:

### Plugin Development
- Each plugin is a separate class with clearly defined responsibilities
- Methods are decorated with `[KernelFunction]` and `[Description]` attributes
- Proper error handling and validation

### Agent Configuration
- Uses `ChatCompletionAgent` for intelligent behavior
- Comprehensive instructions guide the agent's behavior
- Plugins are registered and made available to the agent

### Best Practices
- Separation of concerns between different plugin responsibilities
- Proper async/await patterns
- Error handling and user feedback
- File system security (path validation, restricted access)

## Security Considerations

- File operations are restricted to specified directories
- Path validation prevents directory traversal attacks
- Error messages don't expose sensitive system information
- TODO items are stored locally in the `.agent` folder

## Troubleshooting

### Common Issues

1. **File access errors**: Ensure the application has proper permissions to read/write files
2. **AI service errors**: Verify your API keys and endpoints are correctly configured
3. **Path issues**: Use absolute paths or ensure relative paths are correct

### Debug Mode
Set logging level to `Debug` in the configuration to see detailed operation logs.

## Educational Value

This project demonstrates:
- How to create useful tools for AI agents
- Plugin architecture in Semantic Kernel
- Agent instruction design for specific tasks
- File system operations in a secure manner
- Planning and task management in AI agents
- Integration of multiple capabilities into a cohesive agent

## Contributing

This is a sample project for educational purposes. Feel free to extend it with additional features such as:
- Advanced Gherkin syntax support
- Integration with testing frameworks
- Version control integration
- Template-based feature file generation
- Multi-language support for documentation

## License

This project is part of the "Building effective LLM-based applications with Semantic Kernel" book samples.
