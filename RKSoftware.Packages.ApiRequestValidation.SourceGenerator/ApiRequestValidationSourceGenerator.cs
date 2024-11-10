using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace RKSoftware.Packages.ApiRequestValidation.SourceGenerator;

[Generator]
public class ApiRequestValidationSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<ValidatorToRegister?> itemToGenerate = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => IsSyntaxTargetForGeneration(s), // select validator classes
                    transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx)) // select validator classes and extract details
                .Where(static m => m != null); // Filter out errors that we don't care about

        var itemsToGenerate = itemToGenerate.Collect();

        // Generate source code for each class found
        context.RegisterSourceOutput(itemsToGenerate, static (spc, source) => Execute(source, spc));
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode node)
    {
        if( node is ClassDeclarationSyntax m && m.BaseList != null)
        {
            return m.BaseList.Types.Any(x => x.ToString().Contains("AbstractValidator"));
        }
        return false;
    }

    private static ValidatorToRegister? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        // we know the node is a ClassDeclarationSyntax thanks to IsSyntaxTargetForGeneration
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

        if (context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax) is not INamedTypeSymbol classSymbol)
        {
            // something went wrong
            return null;
        }

        var baseType = classSymbol.BaseType;
        if (baseType == null ||
            !baseType.IsGenericType ||
            !baseType.Name.Equals("AbstractValidator", StringComparison.OrdinalIgnoreCase))
        {
            // we didn't find the validator we were looking for
            return null;
        }

        var classNameToValidate = baseType.TypeArguments[0].OriginalDefinition.ToDisplayString();

        var validatorName = classSymbol.OriginalDefinition.ToDisplayString();

        // we didn't find the validator we were looking for
        return new ValidatorToRegister(classNameToValidate, validatorName);
    }

    private static void Execute(ImmutableArray<ValidatorToRegister?> itemsToGenerate, SourceProductionContext context)
    {
        if (itemsToGenerate != null)
        {
            // generate the source code and add it to the output
            var result = ApiRequestValidationSourceGeneratorHelper.GenerateValidatorRegistrationsClass(itemsToGenerate);

            // Create a separate partial class file for each enum
            context.AddSource($"ValidatorRegistrations.g.cs", SourceText.From(result, Encoding.UTF8));
        }
    }
}
