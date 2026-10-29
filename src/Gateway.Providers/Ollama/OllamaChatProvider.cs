using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Gateway.Core.Chat;

namespace Gateway.Providers.Ollama;

internal sealed class OllamaChatProvider(HttpClient http) : IChatProvider
{
    public async Task<ChatCompletionResponse> CompleteAsync(ChatCompletionRequest request, CancellationToken ct)
    {
        var body = new OllamaChatRequest(
            request.Model,
            request.Messages,
            Stream: false,
            new OllamaOptionsPayload(request.Temperature, request.MaxTokens));

        OllamaChatResponse? reply;
        try
        {
            using var response = await http.PostAsJsonAsync("api/chat", body, ct);
            response.EnsureSuccessStatusCode();
            reply = await response.Content.ReadFromJsonAsync<OllamaChatResponse>(ct);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException && !ct.IsCancellationRequested)
        {
            throw new ProviderException("Ollama request failed.", ex);
        }

        if (reply?.Message is null)
            throw new ProviderException("Ollama returned an empty response.");

        return new ChatCompletionResponse(
            Id: $"chatcmpl-{Guid.NewGuid():N}",
            Object: "chat.completion",
            Created: DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Model: reply.Model ?? request.Model,
            Choices: [new ChatChoice(0, reply.Message, reply.DoneReason ?? "stop")],
            Usage: new ChatUsage(reply.PromptEvalCount, reply.EvalCount, reply.PromptEvalCount + reply.EvalCount));
    }

    private sealed record OllamaChatRequest(
        [property: JsonPropertyName("model")] string Model,
        [property: JsonPropertyName("messages")] IReadOnlyList<ChatMessage> Messages,
        [property: JsonPropertyName("stream")] bool Stream,
        [property: JsonPropertyName("options")] OllamaOptionsPayload Options);

    private sealed record OllamaOptionsPayload(
        [property: JsonPropertyName("temperature"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] double? Temperature,
        [property: JsonPropertyName("num_predict"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] int? NumPredict);

    private sealed record OllamaChatResponse(
        [property: JsonPropertyName("model")] string? Model,
        [property: JsonPropertyName("message")] ChatMessage? Message,
        [property: JsonPropertyName("done_reason")] string? DoneReason,
        [property: JsonPropertyName("prompt_eval_count")] int PromptEvalCount,
        [property: JsonPropertyName("eval_count")] int EvalCount);
}
