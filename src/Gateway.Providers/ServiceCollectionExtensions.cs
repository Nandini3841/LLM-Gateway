using Gateway.Core.Chat;
using Gateway.Providers.Ollama;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOllamaProvider(this IServiceCollection services, IConfiguration configuration)
    {
        var baseUrl = configuration["Providers:Ollama:BaseUrl"] ??
                      throw new InvalidOperationException("Ollama baseUrl isn't configured");
        services.AddHttpClient<IChatProvider, OllamaChatProvider>(client => client.BaseAddress = new Uri(baseUrl));
        return services;
    }
}