using Dapper;

namespace ReportService.DataAccess;

public interface IEmployeeRepository
{
    Task<EmployeeDbo[]> GetAllAsync(CancellationToken ct);
}

public class EmployeeRepository : IEmployeeRepository
{
    private readonly ReportDbContext _dbContext;

    public EmployeeRepository(ReportDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<EmployeeDbo[]> GetAllAsync(CancellationToken ct)
    {
        var result = await _dbContext.Connection.QueryAsync<EmployeeDbo>(
            @"SELECT e.name, e.inn, d.name AS department 
                 FROM emps e
                 JOIN deps d ON e.departmentid = d.id AND d.active = true", ct);
        return result.ToArray();
    }
}