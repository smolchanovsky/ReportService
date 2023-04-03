using System.ComponentModel.DataAnnotations;

namespace SalaryService.Client;

public class SalaryServiceClientConfig
{
    public const string SectionPath = "SalaryServiceClient";

    [Url]
    public string BaseAddress { get; init; } = default!;
}