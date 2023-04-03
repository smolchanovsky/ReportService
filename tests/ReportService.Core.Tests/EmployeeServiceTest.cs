using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Common.Testing;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using ReportService.DataAccess;
using SalaryService.Client;

namespace ReportService.Core.Tests;

[Parallelizable]
public class EmployeeServiceTest
{
    [Test, AutoInject]
    public async Task GetAllAsync_ShouldReturnEmployees_WhenRepositoryHasData(
        [Frozen] Mock<IEmployeeRepository> employeeRepositoryMock,
        [Frozen] Mock<IBuhServiceClient> buhServiceClientMock,
        [Frozen] Mock<ISalaryServiceClient> salaryServiceClientMock,
        EmployeeDbo[] employeeDbos,
        string buhCodeResponse,
        decimal salaryResponse,
        EmployeeService sut)
    {
        // Arrange
        employeeRepositoryMock
            .Setup(x => x
                .GetAllAsync(CancellationToken.None))
            .ReturnsAsync(employeeDbos);

        buhServiceClientMock
            .Setup(x => x
                .GetBuhCodeAsync(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(buhCodeResponse);

        salaryServiceClientMock
            .Setup(x => x
                .GetSalaryAsync(It.IsAny<string>(), It.IsAny<SalaryRequest>(), CancellationToken.None))
            .ReturnsAsync(salaryResponse);

        var expectedEmployees = employeeDbos
            .Select(dbo => new Employee(
                Name: dbo.Name,
                Department: dbo.Department,
                Salary: salaryResponse));

        // Act
        var result = await sut.GetAllAsync(CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedEmployees);
    }
    
    [Test, AutoInject]
    public async Task GetAllAsync_ShouldThrowException_WhenBuhServiceClientReturnsError(
        [Frozen] Mock<IEmployeeRepository> employeeRepositoryMock,
        [Frozen] Mock<IBuhServiceClient> buhServiceClientMock,
        EmployeeDbo[] employeeDbos,
        EmployeeService sut)
    {
        // Arrange
        employeeRepositoryMock
            .Setup(x => x
                .GetAllAsync(CancellationToken.None))
            .ReturnsAsync(employeeDbos);

        buhServiceClientMock
            .Setup(x => x
                .GetBuhCodeAsync(It.IsAny<string>(), CancellationToken.None))
            .Throws<HttpRequestException>();

        // Act
        Func<Task> action = async () => await sut.GetAllAsync(CancellationToken.None);

        // Assert
        await action.Should().ThrowExactlyAsync<HttpRequestException>();
    }
    
    [Test, AutoInject]
    public async Task GetAllAsync_ShouldThrowException_WhenSalaryServiceClientReturnsError(
        [Frozen] Mock<IEmployeeRepository> employeeRepositoryMock,
        [Frozen] Mock<IBuhServiceClient> buhServiceClientMock,
        [Frozen] Mock<ISalaryServiceClient> salaryServiceClientMock,
        EmployeeDbo[] employeeDbos,
        string buhCodeResponse,
        EmployeeService sut)
    {
        // Arrange
        employeeRepositoryMock
            .Setup(x => x
                .GetAllAsync(CancellationToken.None))
            .ReturnsAsync(employeeDbos);
        
        buhServiceClientMock
            .Setup(x => x
                .GetBuhCodeAsync(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(buhCodeResponse);

        salaryServiceClientMock
            .Setup(x => x
                .GetSalaryAsync(It.IsAny<string>(), It.IsAny<SalaryRequest>(), CancellationToken.None))
            .Throws<HttpRequestException>();

        // Act
        Func<Task> action = async () => await sut.GetAllAsync(CancellationToken.None);
        
        // Assert
        await action.Should().ThrowExactlyAsync<HttpRequestException>();
    }
}