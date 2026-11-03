namespace Gateway.Core.Pipeline;

public class StageResult
{
    public StageResultEnum Kind { get; }
    public int StatusCode { get; private init; }
    public ErrorResponse? Error { get; private init; }
    public PromptResponse? Response { get; private init; }

    private StageResult(StageResultEnum kind)
    {
        Kind = kind;
    }

    public static StageResult Continue() => new(StageResultEnum.Continue);

    public static StageResult Fail(int statusCode, ErrorResponse error) => new(StageResultEnum.Fail)
        { StatusCode = statusCode, Error = error };

    public static StageResult Respond(PromptResponse response) => new(StageResultEnum.Respond)
        { Response = response };
}

public enum StageResultEnum
{
    Continue = 1,
    Fail = 2,
    Respond = 3,
}