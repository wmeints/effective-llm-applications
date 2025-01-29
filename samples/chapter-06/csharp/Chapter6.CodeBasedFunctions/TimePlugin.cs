using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace Chapter6.CodeBasedFunctions;

public class TimePlugin
{
    [KernelFunction("get_current_time")]
    [Description("Get the current date and time.")]
    public string GetCurrentTime()
    {
        return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}