using System.Globalization;

namespace ReportService.Core;

public interface IEmployeeReportFormatter
{
    Stream Format(DateTime date, ICollection<Employee> employees, CultureInfo cultureInfo);
}

public class EmployeeReportFormatter : IEmployeeReportFormatter
{
    private readonly string _delimiter = new('-', count: 20);
    private readonly string _currencySymbol = "руб.";
    
    public Stream Format(DateTime date, ICollection<Employee> employees, CultureInfo cultureInfo)
    {
        using var streamWriter = new StreamWriter(new MemoryStream(), leaveOpen: true);
        
        streamWriter.WriteLine(date.ToString(format: "Y", cultureInfo));
        
        foreach (var departmentEmployees in employees.GroupBy(x => x.Department))
        {
            streamWriter.WriteLine(_delimiter);
            streamWriter.WriteLine(departmentEmployees.Key);
            
            foreach (var employee in departmentEmployees)
            {
                streamWriter.WriteLine($"{employee.Name}\t{FormatCurrencyAmount(employee.Salary)}");
            }
            
            var departmentSum = departmentEmployees.Sum(x => x.Salary);
            streamWriter.WriteLine($"Всего по отделу\t{FormatCurrencyAmount(departmentSum)}");
        }
        
        streamWriter.WriteLine(_delimiter);
        
        var totalSum = employees.Sum(x => x.Salary);
        streamWriter.WriteLine($"Всего по предприятию\t{FormatCurrencyAmount(totalSum)}");

        streamWriter.Flush();
        streamWriter.BaseStream.Position = 0;
        return streamWriter.BaseStream;
    }

    private string FormatCurrencyAmount(decimal amount) => 
        $"{amount} {_currencySymbol}";
}