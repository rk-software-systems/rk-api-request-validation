using FluentValidation;
using FluentValidation.Results;

namespace RKSoftware.Packages.ApiRequestValidation;

/// <summary>
/// Validation processor interface
/// </summary>
public interface IValidationProcessor
{

    /// <summary>
    /// Validate object
    /// </summary>
    /// <param name="classNameToValidate"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    Task<ValidationResult?> Validate(string classNameToValidate, object obj);
}
