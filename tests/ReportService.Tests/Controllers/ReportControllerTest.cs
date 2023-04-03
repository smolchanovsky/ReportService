using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Common.Testing;
using FluentAssertions;
using FluentAssertions.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using ReportService.DataAccess;
using SalaryService.Client;

namespace ReportService.Tests.Controllers
{
    public class ReportControllerTests
    {
        private Mock<IEmployeeRepository> _employeeRepositoryMock = default!;
        private Mock<IBuhServiceClient> _buhServiceClientMock = default!;
        private Mock<ISalaryServiceClient> _salaryServiceClientMock = default!;
        private WebApplicationFactory<Program> _factory = default!;
        private HttpClient _client = default!;

        [SetUp]
        public void SetUp()
        {
            _employeeRepositoryMock = new Mock<IEmployeeRepository>();
            
            _buhServiceClientMock = new Mock<IBuhServiceClient>();
            _buhServiceClientMock
                .Setup(x => x
                    .GetBuhCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("buh-code");
            
            _salaryServiceClientMock = new Mock<ISalaryServiceClient>();
            _salaryServiceClientMock
                .Setup(x => x.
                    GetSalaryAsync(It.IsAny<string>(), It.IsAny<SalaryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(100);

            _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddScoped(_ => _employeeRepositoryMock.Object);
                    services.AddScoped(_ => _buhServiceClientMock.Object);
                    services.AddScoped(_ => _salaryServiceClientMock.Object);
                });
            });
            _client = _factory.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            _factory.Dispose();
        }

        [Test]
        public async Task Download_ValidRequest_ReturnsFile()
        {
            // Arrange
            var date = DateTime.Parse("2023-04-02 10:30:00");
            const string expectedContentType = "text/plain";
            const string expectedFileName = "report.txt";
            const string expectedReportContent = 
@"2023 April
--------------------
Department-1
Name-1	100 руб.
Name-2	100 руб.
Всего по отделу	200 руб.
--------------------
Department-2
Name-3	100 руб.
Всего по отделу	100 руб.
--------------------
Всего по предприятию	300 руб.
";
            
            _employeeRepositoryMock
                .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[]
                {
                    new EmployeeDbo("Name-1", "Department-1", "Inn-1"),
                    new EmployeeDbo("Name-2", "Department-1", "Inn-2"),
                    new EmployeeDbo("Name-3", "Department-2", "Inn-3"),
                });

            // Act
            var response = await _client.GetAsync($"/api/report/{date.Year}/{date.Month}", CancellationToken.None);

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Should().NotBeNull();
            response.Content.Headers.ContentType!.MediaType.Should().Be(expectedContentType);
            response.Content.Headers.ContentDisposition!.FileName.Should().Be(expectedFileName);
            var reportContent = await response.Content.ReadAsStringAsync();
            reportContent.Should().Be(expectedReportContent);

        }
        
        [Test, AutoInject]
        public async Task Download_InvalidValidRequestYearAndMonth_ReturnsValidationProblemDetails()
        {
            // Arrange
            const string expectedContentType = "application/problem+json";
            const string expectedValidationProblemJson = @"
{
    ""title"": ""One or more validation errors occurred."",
    ""status"": 400,
    ""detail"": ""See validationErrors for details."",
    ""errors"": {
        ""Year"": [
            ""Invalid value for the year. The year must be between 1 and 9999.""
        ],
        ""Month"": [
            ""Invalid value for the month. The month must be between 1 and 12.""
        ]
    }
}
";
            var expectedValidationProblem = JObject.Parse(expectedValidationProblemJson);
            
            // Act
            var response = await _client.GetAsync($"/api/report/{0}/{13}", CancellationToken.None);

            // Assert
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.Content.Should().NotBeNull();
            response.Content.Headers.ContentType!.MediaType.Should().Be(expectedContentType);
            var validationProblemJson = await response.Content.ReadAsStringAsync();
            validationProblemJson.Should().BeValidJson();
            JObject.Parse(validationProblemJson).Should().BeEquivalentTo(expectedValidationProblem);
        }
    }
}