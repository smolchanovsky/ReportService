using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReportService.Core;
using ReportService.Extensions;
using ReportService.Validation;

namespace ReportService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportController : ControllerBase
{
    private readonly IReportDownloadRequestValidator _downloadRequestValidator;
    private readonly IEmployeeService _employeeService;
    private readonly IEmployeeReportFormatter _employeeReportFormatter;

    public ReportController(
        IReportDownloadRequestValidator downloadRequestValidator,
        IEmployeeService employeeService,
        IEmployeeReportFormatter employeeReportFormatter)
    {
        _downloadRequestValidator = downloadRequestValidator;
        _employeeService = employeeService;
        _employeeReportFormatter = employeeReportFormatter;
    }
    
    // NOTE: There is no reason to pass the year and month here,
    // because they are not used for the filtration in the SQL query.
    // However, for backward compatibility, I will leave these parameters.
    [HttpGet("{year}/{month}")]
    public async Task<IActionResult> Download(int year, int month, CancellationToken ct)
    {
        var validationResult = _downloadRequestValidator.Validate((year, month));
        if (!validationResult.IsValid) 
            return BadRequest(validationResult.ToValidationProblemDetails());

        // TODO: It's correct to output the current date. But it's not done for backward compatibility yet. See Jira Task-123.
        var date = new DateTime(year, month, day: 1);
        var employees = await _employeeService.GetAllAsync(ct);
        var report = _employeeReportFormatter.Format(date, employees, CultureInfo.CurrentCulture);
        
        return File(report, contentType: "text/plain", fileDownloadName: "report.txt");
    }
}