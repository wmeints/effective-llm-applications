using Microsoft.SemanticKernel;

namespace Chapter1.Console;

public class ProductNameGenerator(Kernel kernel)
{
    public async Task<string> GenerateProductNames()
    {
        var productNames = await kernel.InvokePromptAsync("Generate 5 product names for a new line of shoes.");
        return productNames.GetValue<string>()!;
    }
}