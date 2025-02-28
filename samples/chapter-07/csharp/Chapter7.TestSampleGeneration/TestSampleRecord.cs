
using System.Text;

namespace Chapter7.TestSampleGeneration;
public class TestSampleRecord(string question, string answer, List<TextUnit> context)
{
    public string Question => question;
    public string Answer => answer;
    public string Context => FormatContext(context);

    private string FormatContext(List<TextUnit> context)
    {
        var outputBuilder = new StringBuilder();

        return outputBuilder.ToString();
    }
}
