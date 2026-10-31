using Gateway.Core;
using Gateway.Core.Chat;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.API.Endpoints;

[ApiController]
[Route("v1")]
public class ChatController(IChatProvider provider) : ControllerBase
{
    [HttpPost("chat/completions")]
    public async Task<IActionResult> GetPromptResponse(PromptRequest req, CancellationToken ct)
    {
        if (req.Messages.Count == 0) return InvalidRequest("Messages must not be empty");
        if (req.Stream) return InvalidRequest("Streaming is not supported yet.");
        return Ok(await provider.CompleteAsync(req, ct));
    }

    private IActionResult InvalidRequest(string message) => BadRequest(new ErrorResponse
    {
        Error = new Error
        {
            Message = message, Type = "invalid_request_error"
        }
    });
}