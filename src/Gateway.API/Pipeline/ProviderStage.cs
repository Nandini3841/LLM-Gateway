using Gateway.Core;
using Gateway.Core.Chat;
using Gateway.Core.Pipeline;

namespace Gateway.API.Pipeline;

public class ProviderStage(IChatProvider provider) : IPipelineStage
{
    public async Task<StageResult> ExecutePipelineAsync(PipelineContext context, CancellationToken ct)
    {
        PromptResponse response;
        try
        {
            response = await provider.CompleteAsync(context.InfoRequest.Request, ct);
        }
        catch
        {
            context.InfoResponse.Outcome = OutcomeEnum.Failure;
            throw;
        }

        context.InfoResponse.Response = response;
        context.InfoResponse.Model = response.Model;
        context.InfoResponse.Outcome = OutcomeEnum.Success;
        context.InfoResponse.TokenCount = response.Usage;
        return StageResult.Continue();
    }
}