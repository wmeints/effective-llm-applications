
using System.Text;

namespace Chapter7.TestSampleGeneration;

public record TestSampleRecord(string Question, string Answer, List<TextUnit> Context);