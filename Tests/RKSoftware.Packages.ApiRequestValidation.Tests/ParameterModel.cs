using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RKSoftware.Packages.ApiRequestValidation.Tests
{
    internal class ParameterModel
    {
        internal ParameterModel(string name, 
            BindingSource bindingSource, 
            Object? value)
        {
            Name = name;
            BindingSource = bindingSource;
            Value = value;
            Type = value?.GetType();
        }   


        public string Name { get; }

        public BindingSource BindingSource { get; }

        public object? Value { get; }

        public Type? Type { get; }
    }
}
