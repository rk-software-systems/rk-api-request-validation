# Automatic validation for API request parameters (.NET)
Repository contains functionality for validation API requests.

| Package  |  Current version and Downloads
|---|---|
|  RKSoftware.Packages.ApiRequestValidation  | [![NuGet Badge](https://buildstats.info/nuget/RKSoftware.Packages.ApiRequestValidation)](https://www.nuget.org/packages/RKSoftware.Packages.ApiRequestValidation/)

## About
This package contains `ActionExecutingContext` extensions for validation URL parameters together with Body, Form and Query model validation.
- If an `id` in URL `posts/{id}/pages` is empty, an endpoint returns Not Found response.
- If a model next to `[FromBody]`, `[FromForm]` or `[FromQuery]` is not valid, an endpoint returns Bad Request response with `ValidationProblemDetails` model.
It uses `FluentValidation` (https://github.com/FluentValidation/FluentValidation) for validating models.

## How To Use

- Create `FluentValidation` model validator in accordance with documentation https://docs.fluentvalidation.net/en/latest/start.html.

- Register created validator
```

builder.Services.AddScoped<IValidator<BodyModel>, BodyModelValidator>();

```
or register all created validators from the same assembly
```
builder.Services.AddValidatorsFromAssemblyContaining<BodyModelValidator>();
```

- The package contains `ApiRequestValidation` attribute, which can be set on a controller or its method.
```
[ApiRequestValidation]
public class MyController : Controller
{
}
```
or
```

public class MyController : Controller
{

    [ApiRequestValidation]
	[HttpPost("posts/{id}/pages")]   
    public async Task<IActionResult> Create(string id, [FromBody] BodyModel body, [FromQuery] QueryModel query)
    {
        ...
    }

}
```