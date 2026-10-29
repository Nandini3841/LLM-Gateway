using Gateway.Core.Chat;
using Gateway.Providers.Ollama;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;

namespace Gateway.Providers;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOllamaProvider(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<OllamaOptions>()
            .Bind(config.GetSection(OllamaOptions.SectionName))
            .Validate(o => o.BaseUrl.IsAbsoluteUri, "Providers:Ollama:BaseUrl must be an absolute URL.")
            .ValidateOnStart();

        services.AddHttpClient<IChatProvider, OllamaChatProvider>((sp, client) =>
            {
                client.BaseAddress = sp.GetRequiredService<IOptions<OllamaOptions>>().Value.BaseUrl;
            })
            .AddStandardResilienceHandler(o =>
            {
                // Local models are slow: first-token load can take a while.
                o.AttemptTimeout.Timeout = TimeSpan.FromSeconds(120);
                o.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(300);
                o.Retry.DisableForUnsafeHttpMethods();
                o.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(240);
            });

        return services;
    }
}
