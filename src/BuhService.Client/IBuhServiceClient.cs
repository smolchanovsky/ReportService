using Refit;

namespace SalaryService.Client;

public interface IBuhServiceClient
{
    [Post("/inn/{inn}")]
    Task<string> GetBuhCodeAsync(string inn, CancellationToken ct);
}