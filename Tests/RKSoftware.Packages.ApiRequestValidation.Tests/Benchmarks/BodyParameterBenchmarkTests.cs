using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Xunit.Abstractions;
using RKSoftware.Packages.ApiRequestValidation.Fakes;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RKSoftware.Packages.ApiRequestValidation.Tests.Benchmarks;

public class BodyParameterBenchmarkTests(ITestOutputHelper output)
{
    private readonly ITestOutputHelper output = output;

    [Fact]
    public void RunBenchmarks()
    {
        var logger = new AccumulationLogger();

        var config = ManualConfig.Create(DefaultConfig.Instance)
            .AddLogger(logger);

        BenchmarkRunner.Run<BodyParameterBenchmarks>(config);

        // write benchmark summary
        output.WriteLine(logger.GetLog());
    }
}

[MemoryDiagnoser]
public class BodyParameterBenchmarks
{
    private const int N = 100;

    private static readonly List<ParameterModel<FakeGodInputModel>> _parameters = new()
    {
        new("modelBody", BindingSource.Body, new FakeGodInputModel
        {
            SystemName = null
        })
    };

    private static readonly List<ParameterModel<FakeGodInputStructModel>> _structParameters = new()
    {
        new("modelBody", BindingSource.Body, new FakeGodInputStructModel
        {
            SystemName = null
        })
    };

    [Benchmark(Baseline = true)]
    public async Task TestRequestWhenBodyHasEmptyRequiredProperty()
    {
        ActionExecutingContext? actionExecutingContext = null;
        for (int i = 0; i < N; i++)
        {
            actionExecutingContext = await ActionExecutingContextHelper.GetActionExecutingContext(_parameters, new ApiRequestValidationAttribute());
            
        }
        Assert.False(actionExecutingContext?.ModelState.IsValid);
    }

    [Benchmark]
    public async Task TestRequestWhenBodyHasEmptyRequiredPropertySg()
    {
        ActionExecutingContext? actionExecutingContext = null;
        for (int i = 0; i < N; i++)
        {
            actionExecutingContext = await ActionExecutingContextHelper.GetActionExecutingContextSg(_parameters, new ApiRequestValidationSgAttribute());
            
        }
        Assert.False(actionExecutingContext?.ModelState.IsValid);
    }

    [Benchmark]
    public async Task TestRequestWhenBodyHasEmptyRequiredPropertyStructure()
    {
        ActionExecutingContext? actionExecutingContext = null;
        for (int i = 0; i < N; i++)
        {
           actionExecutingContext = await ActionExecutingContextHelper.GetActionExecutingContext(_structParameters, new ApiRequestValidationAttribute());           
        }
        Assert.False(actionExecutingContext?.ModelState.IsValid);
    }

    [Benchmark]
    public async Task TestRequestWhenBodyHasEmptyRequiredPropertySgStructure()
    {
        ActionExecutingContext? actionExecutingContext = null;
        for (int i = 0; i < N; i++)
        {
            actionExecutingContext = await ActionExecutingContextHelper.GetActionExecutingContextSg(_structParameters, new ApiRequestValidationSgAttribute());            
        }
        Assert.False(actionExecutingContext?.ModelState.IsValid);
    }
}
