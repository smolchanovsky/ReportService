using System;
using FluentValidation;

namespace ReportService.Validation;

public interface IReportDownloadRequestValidator : IValidator<(int Year, int Month)>
{
}

public class ReportDownloadRequestValidator : AbstractValidator<(int Year, int Month)>, IReportDownloadRequestValidator
{
    public ReportDownloadRequestValidator()
    {
        RuleFor(x => x.Year)
            .InclusiveBetween(DateTime.MinValue.Year, DateTime.MaxValue.Year)
            .OverridePropertyName("Year")
            .WithMessage(
                "Invalid value for the year. " +
                $"The year must be between {DateTime.MinValue.Year} and {DateTime.MaxValue.Year}.");
        
        RuleFor(x => x.Month)
            .InclusiveBetween(DateTime.MinValue.Month, DateTime.MaxValue.Month)
            .OverridePropertyName("Month")
            .WithMessage(
                "Invalid value for the month. " +
                $"The month must be between {DateTime.MinValue.Month} and {DateTime.MaxValue.Month}.");
    }
}