using Microsoft.Extensions.Configuration;

namespace ReportService.Extensions;

public static class ConnectionStringExtensions
{
    public static string GetDefaultConnectionString(this IConfiguration configuration)
    {
        return configuration.GetConnectionString("DefaultConnection");
    }
}