# Chapter 11 - Feature File Agent

This application demonstrates an AI-powered agent that automatically generates Gherkin
feature files for behavior-driven development (BDD) based on project requirements and
documentation. The agent uses Semantic Kernel with Azure OpenAI to understand
requirements and create comprehensive, well-structured feature files.

## Application Flow

The Feature File Agent follows a structured process to generate high-quality Gherkin feature files:

### 1. Input Processing

- **User Input**: The agent accepts either a direct prompt via command line arguments or
  reads requirements from an input file
- **Interactive Mode**: After initial generation, the agent enters an interactive chat
  mode where users can refine requirements or request modifications

### 2. Planning Phase

- **Requirement Analysis**: The agent analyzes the provided requirements to identify key
  scenarios that need to be documented
- **TODO Management**: Creates TODO items for each identified scenario to track progress
  and ensure completeness
- **Documentation Review**: If reference documentation is provided, the agent examines
  it to understand context and examples

### 3. Feature File Generation

For each TODO item (scenario), the agent:

- **Context Gathering**: Identifies relevant examples and patterns from reference documentation
- **Scenario Writing**: Creates detailed Gherkin scenarios with Given/When/Then steps
- **Completeness Check**: Reviews each scenario for clarity and completeness
- **TODO Completion**: Marks the scenario as completed in the TODO list

### 4. Quality Assurance

- **Gap Analysis**: Reviews the entire feature file to identify any missing scenarios
- **Validation**: Scores the feature file on three criteria (1-10 scale):
  - Readability for business users
  - Automation difficulty for developers  
  - Completeness of coverage

### 5. Output Management

- **File Writing**: Saves the generated feature file to the specified output path
- **Streaming Response**: Provides real-time feedback during generation
- **Interactive Refinement**: Allows users to request changes or additions through chat

## Core Components

### Plugins

- **FeatureFilePlugin**: Manages reading and writing feature file content
- **TodoItemsPlugin**: Tracks planning tasks and scenario completion
- **FileSystemPlugin**: Reads reference documentation (when specified)

### Services

- **FeatureFileGenerator**: Orchestrates the AI agent, manages chat history, and handles retry logic for rate limiting

### Filters

- **ToolInvocationFilter**: Monitors and logs function calls made by the agent

## Configuration Setup

### 1. Using a configuration file

Create a configuration file at `.agent/config.local.json`:

```json
{
    "LanguageModel": {
        "Endpoint": "https://your-resource-name.openai.azure.com/",
        "ApiKey": "your-api-key-here",
        "DeploymentName": "your-gpt4-deployment-name"
    }
}
```

**Configuration Options:**
- **Endpoint**: Your Azure OpenAI service endpoint URL
- **ApiKey**: Azure OpenAI API key for authentication
- **DeploymentName**: Name of your GPT-4 deployment in Azure OpenAI

### 2. Alternative configuration methods

#### Using User Secrets (Recommended for Development)

```bash
dotnet user-secrets set "LanguageModel:Endpoint" "https://your-resource-name.openai.azure.com/"
dotnet user-secrets set "LanguageModel:ApiKey" "your-api-key-here"
dotnet user-secrets set "LanguageModel:DeploymentName" "your-gpt4-deployment-name"
```

#### Using Environment Variables

```bash
# Windows PowerShell
$env:LanguageModel__Endpoint = "https://your-resource-name.openai.azure.com/"
$env:LanguageModel__ApiKey = "your-api-key-here"
$env:LanguageModel__DeploymentName = "your-gpt4-deployment-name"

# Linux/macOS
export LanguageModel__Endpoint="https://your-resource-name.openai.azure.com/"
export LanguageModel__ApiKey="your-api-key-here"
export LanguageModel__DeploymentName="your-gpt4-deployment-name"
```
