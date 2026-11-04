using Gateway.Core.Pipeline;

namespace Gateway.API.Pipeline;

public class PipelineRunner(IEnumerable<IPipelineStage> stages, ILogger<PipelineRunner> logger)
{
   public async Task<StageResult> RunAsync(PipelineContext context, CancellationToken ct)
   {
       var ran = new List<IPipelineStage>();
       var result = StageResult.Continue();
       try
       {
           foreach (var stage in stages)
           {
               StageResult r;
               ran.Add(stage);
               try
               {
                   r = await stage.ExecutePipelineAsync(context, ct);
               }
               catch (Exception ex) when (ex is not OperationCanceledException)
               {
                   logger.LogError(ex, "stage {Stage} failed for request {ReqId}",stage.GetType().Name,context.InfoRequest.RequestId);
                   throw;
               }
               if (r.Kind != StageResultEnum.Continue)
               {
                   result = r;
                   break;
               }
           }
       }
       finally
       {
           for (var i = ran.Count - 1; i >= 0; i--)
           {
               try
               {
                   await ran[i].CompletePipelineAsync(context, CancellationToken.None);
               }
               catch(Exception ex)
               {
                   logger.LogWarning(ex, "Cleanup of stage {Stage} failed for request {RequestId}",
                       ran[i].GetType().Name, context.InfoRequest.RequestId);
               }
           }
       }
       return result;
   }
}

