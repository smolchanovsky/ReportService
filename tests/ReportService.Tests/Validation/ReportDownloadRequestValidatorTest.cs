using System;
using Common.Testing;
using FluentAssertions;
using FluentValidation;
using NUnit.Framework;
using ReportService.Validation;

namespace ReportService.Tests.Validation;

[Parallelizable]
public class ReportDownloadRequestValidatorTest
{
    [Test, AutoInject]
    public void Validate_WhenValidRequest_ShouldNotThrowException(
        DateTime date,
        ReportDownloadRequestValidator sut)
    {
        // Arrange
        var request = (date.Year, date.Month);

        // Act
        var action = () => sut.ValidateAndThrow(request);

        // Assert
        action.Should().NotThrow<ValidationException>();
    }

    [Test, AutoInject]
    public void Validate_WhenYearLessThanMinValue_ShouldThrowValidationException(
        ReportDownloadRequestValidator sut)
    {
        // Arrange
        var request = (Year: DateTime.MinValue.Year - 1, DateTime.MinValue.Month);

        // Act
        var action = () => sut.ValidateAndThrow(request);

        // Assert
        action.Should().Throw<ValidationException>()
            .And.Errors.Should().Contain(x => x.PropertyName == nameof(request.Year));
    }

    [Test, AutoInject]
    public void Validate_WhenYearGreaterThanMaxValue_ShouldThrowValidationException(
        ReportDownloadRequestValidator sut)
    {
        // Arrange
        var request = (Year: DateTime.MaxValue.Year + 1, DateTime.MaxValue.Month);

        // Act
        var action = () => sut.ValidateAndThrow(request);

        // Assert
        action.Should().Throw<ValidationException>()
            .And.Errors.Should().Contain(x => x.PropertyName == nameof(request.Year));
    }

    [Test, AutoInject]
    public void Validate_WhenMonthLessThanMinValue_ShouldThrowValidationException(
        ReportDownloadRequestValidator sut)
    {
        // Arrange
        var request = (DateTime.MinValue.Year, Month: DateTime.MinValue.Month - 1);

        // Act
        var action = () => sut.ValidateAndThrow(request);

        // Assert
        action.Should().Throw<ValidationException>()
            .And.Errors.Should().Contain(x => x.PropertyName == nameof(request.Month));
    }

    [Test, AutoInject]
    public void Validate_WhenMonthGreaterThanMaxValue_ShouldThrowValidationException(
        ReportDownloadRequestValidator sut)
    {
        // Arrange
        var request = (DateTime.MaxValue.Year, Month: DateTime.MaxValue.Month + 1);

        // Act
        var action = () => sut.ValidateAndThrow(request);

        // Assert
        action.Should().Throw<ValidationException>()
            .And.Errors.Should().Contain(x => x.PropertyName == nameof(request.Month));
    }
}