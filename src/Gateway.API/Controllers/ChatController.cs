using Gateway.API.Pipeline;
using Gateway.Core;
using Gateway.Core.Pipeline;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.API.Endpoints;

[ApiController]
[Route("v1")]
public class ChatController(PipelineRunner runner) : ControllerBase
{
    [HttpPost("chat/completions")]
    public async Task<IActionResult> GetPromptResponse(PromptRequest req, CancellationToken ct)
    {
        if (req.Messages.Count == 0) return InvalidRequest("Messages must not be empty");
        if (req.Stream) return InvalidRequest("Streaming is not supported yet.");
        var context = new PipelineContext { InfoRequest = new InfoRequest{Request = req} };
        var result = await runner.RunAsync(context, ct);
        return result.Kind switch 
        {
            StageResultEnum.Fail => StatusCode(result.StatusCode,result.Error),
            StageResultEnum.Respond => Ok(result.Response),
            _ => Ok(context.InfoResponse.Response)
        };
    }

    private IActionResult InvalidRequest(string message) => BadRequest(new ErrorResponse
    {
        Error = new Error
        {
            Message = message, Type = "invalid_request_error"
        }
    });
}