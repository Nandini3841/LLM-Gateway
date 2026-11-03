namespace Gateway.Core.Pipeline;

public interface IPipelineStage
{
    Task<StageResult> ExecutePipelineAsync(PipelineContext context, CancellationToken ct);
    Task CompletePipelineAsync(PipelineContext context, CancellationToken ct) => Task.CompletedTask;
}