using ReportService.DataAccess;
using SalaryService.Client;

namespace ReportService.Core;

public interface IEmployeeService
{
    Task<Employee[]> GetAllAsync(CancellationToken ct);
}

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IBuhServiceClient _buhServiceClient;
    private readonly ISalaryServiceClient _salaryServiceClient;

    public EmployeeService(
        IEmployeeRepository employeeRepository,
        IBuhServiceClient buhServiceClient,
        ISalaryServiceClient salaryServiceClient)
    {
        _employeeRepository = employeeRepository;
        _buhServiceClient = buhServiceClient;
        _salaryServiceClient = salaryServiceClient;
    }
    
    public async Task<Employee[]> GetAllAsync(CancellationToken ct)
    {
        var employeeDbos = await _employeeRepository.GetAllAsync(ct);
        var employees = employeeDbos.Select(async x => await BuildEmployeeAsync(x, ct));
        return await Task.WhenAll(employees);
    }

    private async Task<Employee> BuildEmployeeAsync(EmployeeDbo dbo, CancellationToken ct)
    {
        var buhCode = await _buhServiceClient.GetBuhCodeAsync(dbo.Inn, ct);
        var salary = await _salaryServiceClient.GetSalaryAsync(dbo.Inn, new SalaryRequest(buhCode), ct);
                
        return new Employee(
            Name: dbo.Name, 
            Department: dbo.Department,
            Salary: salary);
    }
}