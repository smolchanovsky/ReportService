using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ReportService.DataAccess;

public class ReportDbContext : DbContext
{
    public NpgsqlConnection Connection { get; }
    
    public ReportDbContext(
        DbContextOptions<ReportDbContext> options, 
        NpgsqlConnection connection) 
        : base(options)
    {
        Connection = connection;
    }
}