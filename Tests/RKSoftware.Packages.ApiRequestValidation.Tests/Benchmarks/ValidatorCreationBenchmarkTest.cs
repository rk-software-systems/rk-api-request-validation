using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using RKSoftware.Packages.ApiRequestValidation.Fakes;
using Xunit.Abstractions;

namespace RKSoftware.Packages.ApiRequestValidation.Tests.Benchmarks;

public class ValidatorCreationBenchmarkTest(ITestOutputHelper output)
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
public class ValidatorCreationBenchmarks
{
    private const int N = 100_000;

    private FakeGodInputModelValidator[] _list = new FakeGodInputModelValidator[N];
    private FakeGodInputStructModelValidator[] _listSt = new FakeGodInputStructModelValidator[N];

    [Benchmark(Baseline = true)]
    public void TestCreateObjectValidatorSimply()
    {
        var service = new CustomService();
        for (int i = 0; i < N; i++)
        {
            var validator = new FakeGodInputModelValidator(service);
            _list[i] = validator;
        }
    }

    [Benchmark]
    public void TestCreateObjectValidatorByReflection()
    {
        var service = Activator.CreateInstance<CustomService>();
        for (int i = 0; i < N; i++)
        {
            var validator = (FakeGodInputModelValidator)Activator.CreateInstance(typeof(FakeGodInputModelValidator), service);
            _list[i] = validator;
        }
    }

    [Benchmark]
    public void TestCreateStructureValidatorSimply()
    {
        var service = new CustomService();
        for (int i = 0; i < N; i++)
        {
            var validator = new FakeGodInputStructModelValidator(service);
            _listSt[i] = validator;
        }
    }

    [Benchmark]
    public void TestCreateStructureValidatorByReflection()
    {
        var service = Activator.CreateInstance<CustomService>();
        for (int i = 0; i < N; i++)
        {
            var validator = (FakeGodInputStructModelValidator)Activator.CreateInstance(typeof(FakeGodInputStructModelValidator), service);
            _listSt[i] = validator;
        }
    }
}
