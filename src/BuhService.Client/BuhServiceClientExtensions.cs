using Humanizer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using Refit;

namespace SalaryService.Client;

public static class BuhServiceClientExtensions
{
    public static OptionsBuilder<BuhServiceClientConfig> AddBuhServiceConfig(this IServiceCollection services)
    {
        return services
            .AddOptions<BuhServiceClientConfig>()
            .BindConfiguration(BuhServiceClientConfig.SectionPath);
    }
    
    public static IHttpClientBuilder AddBuhServiceClient(this IServiceCollection services)
    {
        return services.AddRefitClient<IBuhServiceClient>()
            .ConfigureHttpClient((serviceProvider, httpClient) =>
            {
                var config = serviceProvider.GetRequiredService<IOptions<BuhServiceClientConfig>>();
                httpClient.BaseAddress = new Uri(config.Value.BaseAddress);
            })
            .AddPolicyHandler(HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(new[] { 1.Seconds(), 5.Seconds(), 10.Seconds() }));;
    }
}