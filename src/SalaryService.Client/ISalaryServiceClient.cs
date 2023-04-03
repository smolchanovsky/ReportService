using System.Text.Json.Serialization;
using Refit;

namespace SalaryService.Client;

public record SalaryRequest([property: JsonPropertyName("buhCode")] string BuhCode);

public interface ISalaryServiceClient
{
    [Post("/empcode/{inn}")]
    Task<decimal> GetSalaryAsync(string inn, [Body] SalaryRequest request, CancellationToken ct);
}