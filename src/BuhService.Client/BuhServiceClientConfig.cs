using System.ComponentModel.DataAnnotations;

namespace SalaryService.Client;

public class BuhServiceClientConfig
{
    public const string SectionPath = "BuhServiceClient";

    [Url] 
    public string BaseAddress { get; init; } = default!;
}