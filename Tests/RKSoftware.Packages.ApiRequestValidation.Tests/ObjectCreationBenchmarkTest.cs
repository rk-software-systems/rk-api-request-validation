using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using RKSoftware.Packages.ApiRequestValidation.Fakes;
using Xunit.Abstractions;

namespace RKSoftware.Packages.ApiRequestValidation.Tests;

public class ObjectCreationBenchmarkTest(ITestOutputHelper output)
{
    private readonly ITestOutputHelper output = output;

    [Fact]
    public void RunBenchmarks()
    {
        var logger = new AccumulationLogger();

        var config = ManualConfig.Create(DefaultConfig.Instance)
            .AddLogger(logger);

        BenchmarkRunner.Run<ObjectCreationBenchmarks>(config);

        // write benchmark summary
        output.WriteLine(logger.GetLog());
    }
}


[MemoryDiagnoser]
public class ObjectCreationBenchmarks
{
    [Benchmark(Baseline = true)]
    public void TestCreateObjectSimply()
    {
        var service = new CustomService();
        var validator = new FakeGodInputModelValidator(service);
    }

    [Benchmark]
    public void TestCreateObjectByReflection()
    {
        var service = Activator.CreateInstance<CustomService>();
        var validator = (FakeGodInputModelValidator)Activator.CreateInstance(typeof(FakeGodInputModelValidator), service);
    }
}
