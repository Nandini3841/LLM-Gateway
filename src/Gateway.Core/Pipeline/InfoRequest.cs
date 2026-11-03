namespace Gateway.Core.Pipeline;

public class InfoRequest
{
    public Guid RequestId { get; init; } = Guid.NewGuid();
    public required PromptRequest Request { get; init; }
    public DateTimeOffset StartTime { get; init; } = DateTimeOffset.UtcNow;
}

public class InfoResponse
{
    public PromptResponse? Response { get; set; }
    public string? Model { get; set; }
    public Usage? TokenCount { get; set; }
    public OutcomeEnum Outcome { get; set; }
}

public enum OutcomeEnum
{
    Pending,
    Success,
    Failure
}