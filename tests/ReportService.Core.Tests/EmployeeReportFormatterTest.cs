using System;
using System.Globalization;
using System.IO;
using Common.Testing;
using FluentAssertions;
using NUnit.Framework;

namespace ReportService.Core.Tests;

[Parallelizable]
public class EmployeeReportFormatterTest
{
    [Test, AutoInject]
    public void Format_ShouldReturnStreamWithReport_WhenCalled(EmployeeReportFormatter sut)
    {
        // Arrange
        var date = DateTime.Parse("2023-04-02 10:30:00");
        var employees = new[]
        {
            new Employee("Name-1", "Department-1", Salary: 100),
            new Employee("Name-2", "Department-1", Salary: 200),
            new Employee("Name-3", "Department-2", Salary: 300),
        };
        
        // Act
        var result = sut.Format(date, employees, CultureInfo.CurrentCulture);
        
        // Assert
        result.Should().NotBeNull();
        result.CanRead.Should().BeTrue();
        result.Position.Should().Be(0);
        new StreamReader(result).ReadToEnd().Should().Be(
@"2023 April
--------------------
Department-1
Name-1	100 руб.
Name-2	200 руб.
Всего по отделу	300 руб.
--------------------
Department-2
Name-3	300 руб.
Всего по отделу	300 руб.
--------------------
Всего по предприятию	600 руб.
");
    }
}