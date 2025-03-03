using Xunit.Abstractions;

namespace Benday.DemoApp.IntegrationTests;

public class TestClassBase
{
    private readonly ITestOutputHelper _output;

    public TestClassBase(ITestOutputHelper output)
    {
        _output = output;
    }

    public void WriteLine(string message)
    {
        _output.WriteLine(message);
    }
}