using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Xunit.Abstractions;
using RKSoftware.Packages.ApiRequestValidation.Fakes;

namespace RKSoftware.Packages.ApiRequestValidation.Tests;

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
    private static readonly List<ParameterModel<FakeGodInputModel>> _parameters = new()
    {
        new("modelBody", BindingSource.Body, new FakeGodInputModel
        {
            SystemName = null
        }),
        new("modelForm", BindingSource.Form, new FakeGodInputModel
        {
            SystemName = null
        }),
        new("modelQuery", BindingSource.Query, new FakeGodInputModel
        {
            SystemName = null
        })
    };

    [Benchmark(Baseline = true)]
    public async Task TestRequestWhenBodyHasEmptyRequiredProperty()
    {
        var paramaters = _parameters;

        var actionExecutingContext = await ActionExecutingContextHelper.GetActionExecutingContext(paramaters, new ApiRequestValidationAttribute());

        Assert.False(actionExecutingContext.ModelState.IsValid);
    }

    [Benchmark]
    public async Task TestRequestWhenBodyHasEmptyRequiredProperty2()
    {
        var paramaters = _parameters;

        var actionExecutingContext = await ActionExecutingContextHelper.GetActionExecutingContext(paramaters, new ApiRequestValidation2Attribute());

        Assert.False(actionExecutingContext.ModelState.IsValid);
    }
}
