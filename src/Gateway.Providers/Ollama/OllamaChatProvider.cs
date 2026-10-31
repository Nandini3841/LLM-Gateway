using System.Net;
using System.Net.Http.Json;
using Gateway.Core;
using Gateway.Core.Chat;

namespace Gateway.Providers.Ollama;

internal sealed class OllamaChatProvider(HttpClient client) : IChatProvider
{
    public async Task<PromptResponse> CompleteAsync(PromptRequest req, CancellationToken cancellationToken)
    {
        try
        {

            using var response =
                await client.PostAsJsonAsync("v1/chat/completions", req, GatewayJson.Options, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result =
                await response.Content.ReadFromJsonAsync<PromptResponse>(GatewayJson.Options, cancellationToken);
            return result ?? throw new InvalidOperationException("Ollama returned an empty response");
        }
        catch (HttpRequestException ex)
        {
            throw new ProviderException("Ollama is down", ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new ProviderException("Ollama timed out", ex);
        }
    }
}