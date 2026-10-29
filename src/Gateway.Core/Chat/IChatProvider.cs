namespace Gateway.Core.Chat;

public interface IChatProvider
{
    Task<ChatCompletionResponse> CompleteAsync(ChatCompletionRequest request, CancellationToken ct);
}

/// <summary>The upstream provider failed or was unreachable.</summary>
public sealed class ProviderException(string message, Exception? inner = null) : Exception(message, inner);
