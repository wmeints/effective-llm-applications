using Chapter11.FeatureFileAgent.Commands;
using Spectre.Console.Cli;

var app = new CommandApp<GenerateFeatureFileCommand>();

await app.RunAsync(args);