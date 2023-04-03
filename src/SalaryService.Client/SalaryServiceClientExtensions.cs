using Humanizer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using Refit;

namespace SalaryService.Client;

public static class SalaryServiceClientExtensions
{
    public static OptionsBuilder<SalaryServiceClientConfig> AddSalaryServiceConfig(this IServiceCollection services)
    {
        return services
            .AddOptions<SalaryServiceClientConfig>()
            .BindConfiguration(SalaryServiceClientConfig.SectionPath);
    }
    
    public static IHttpClientBuilder AddSalaryServiceClient(this IServiceCollection services)
    {
        return services.AddRefitClient<ISalaryServiceClient>()
            .ConfigureHttpClient((serviceProvider, httpClient) =>
            {
                var config = serviceProvider.GetRequiredService<IOptions<SalaryServiceClientConfig>>();
                httpClient.BaseAddress = new Uri(config.Value.BaseAddress);
            })
            .AddPolicyHandler(HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(new[] { 1.Seconds(), 5.Seconds(), 10.Seconds() }));
    }
}