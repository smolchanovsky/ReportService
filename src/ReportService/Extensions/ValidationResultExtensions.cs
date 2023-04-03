using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace ReportService.Extensions;

public static class ValidationResultExtensions
{
    public static ValidationProblemDetails ToValidationProblemDetails(this ValidationResult validationResult)
    {
        return new ValidationProblemDetails(validationResult.ToDictionary())
        {
            Status = 400,
            Title = "One or more validation errors occurred.",
            Detail = "See validationErrors for details.",
        };
    }
}