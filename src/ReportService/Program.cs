using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Npgsql;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using ReportService.Core;
using ReportService.DataAccess;
using ReportService.Extensions;
using ReportService.Validation;
using SalaryService.Client;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((_, _, config) => config
    .ReadFrom.Configuration(builder.Configuration));

var services = builder.Services;

services.AddLogging(x => x.ClearProviders().AddSerilog());

services.AddOpenTelemetry()
    .ConfigureResource(config => config
        .AddService("ReportService"))
    .WithTracing(config => config
        .AddSource()
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation());

services.AddHealthChecks().ForwardToPrometheus();

services.AddControllers();

services.AddDbContext<ReportDbContext>((serviceProvider, options) =>
    options.UseNpgsql(serviceProvider.GetRequiredService<IConfiguration>().GetDefaultConnectionString()));
services.AddScoped(serviceProvider => new NpgsqlConnection(
    serviceProvider.GetRequiredService<IConfiguration>().GetDefaultConnectionString()));
services.AddScoped<IEmployeeRepository, EmployeeRepository>();

services.AddBuhServiceConfig().ValidateDataAnnotations().ValidateOnStart();
services.AddBuhServiceClient();

services.AddSalaryServiceConfig().ValidateDataAnnotations().ValidateOnStart();
services.AddSalaryServiceClient();

services.AddSingleton<IReportDownloadRequestValidator, ReportDownloadRequestValidator>();
services.AddSingleton<IEmployeeReportFormatter, EmployeeReportFormatter>();
services.AddScoped<IEmployeeService, EmployeeService>();

services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(name: "v1", new OpenApiInfo { Title = "Report API", Version = "v1" });
});

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseHttpLogging();
app.UseHttpMetrics();

app.UseSwagger();
app.UseSwaggerUI(opts =>
{
    opts.SwaggerEndpoint(url: "/swagger/v1/swagger.json", name: "Report API V1");
});

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHealthChecks("/healthz");
    endpoints.MapMetrics();
    endpoints.MapSwagger();
});

await app.RunAsync();

// HACK: For integration tests
public partial class Program { }