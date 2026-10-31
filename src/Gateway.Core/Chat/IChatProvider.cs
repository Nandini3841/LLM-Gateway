namespace Gateway.Core.Chat;

public interface IChatProvider
{
    Task<PromptResponse> CompleteAsync(PromptRequest req, CancellationToken cancellationToken);
}

