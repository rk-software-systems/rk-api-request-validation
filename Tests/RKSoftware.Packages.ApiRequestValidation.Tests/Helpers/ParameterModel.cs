using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RKSoftware.Packages.ApiRequestValidation.Tests;

internal class ParameterModel<T>
{
    internal ParameterModel(string name, 
        BindingSource bindingSource, 
        T value)
    {
        Name = name;
        BindingSource = bindingSource;
        Value = value;
        Type =typeof(T);
    }   


    public string Name { get; }

    public BindingSource BindingSource { get; }

    public object? Value { get; }

    public Type Type { get; }
}
