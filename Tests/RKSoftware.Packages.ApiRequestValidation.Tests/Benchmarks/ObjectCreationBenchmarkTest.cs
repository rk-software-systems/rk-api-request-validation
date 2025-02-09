using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using RKSoftware.Packages.ApiRequestValidation.Fakes;
using Xunit.Abstractions;

namespace RKSoftware.Packages.ApiRequestValidation.Tests.Benchmarks;

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
    private const int N = 100_000;

    private FakeGodInputModel[] _list = new FakeGodInputModel[N];
    private FakeGodInputStructModel[] _listSt = new FakeGodInputStructModel[N];

    [Benchmark(Baseline = true)]
    public void TestCreateObjectSimply()
    {
        for (int i = 0; i < N; i++)
        {
            var obj = new FakeGodInputModel();
            _list[i] = obj;
        }
    }

    [Benchmark]
    public void TestCreateObjectByReflection()
    {
        for (int i = 0; i < N; i++)
        {
            var obj = Activator.CreateInstance<FakeGodInputModel>();
            _list[i] = obj;
        }
    }

    [Benchmark]
    public void TestCreateStructureSimply()
    {
        for (int i = 0; i < N; i++)
        {
            var st = new FakeGodInputStructModel();
            _listSt[i] = st;
        }
    }

    [Benchmark]
    public void TestCreateStructureByReflection()
    {
        for (int i = 0; i < N; i++)
        {
            var st = Activator.CreateInstance<FakeGodInputStructModel>();
            _listSt[i] = st;
        }
    }
}
