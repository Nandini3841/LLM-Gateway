using Gateway.Core.Chat;

namespace Gateway.API.Endpoints;

public static class ChatEndpoints
{
    public static IEndpointRouteBuilder MapChatEndpoints(this IEndpointRouteBuilder app)
    {
        var v1 = app.MapGroup("/v1");

        v1.MapPost("/chat/completions", async (ChatCompletionRequest request, IChatProvider provider, CancellationToken ct) =>
        {
            if (request.Messages.Count == 0)
                return Results.Problem("`messages` must not be empty.", statusCode: StatusCodes.Status400BadRequest);

            // SSE streaming arrives in a later milestone.
            if (request.Stream)
                return Results.Problem("Streaming is not supported yet.", statusCode: StatusCodes.Status400BadRequest);

            return Results.Ok(await provider.CompleteAsync(request, ct));
        });

        return app;
    }

    /// <summary>Control-plane routes live under /admin so they can be split out later.</summary>
    public static IEndpointRouteBuilder MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGroup("/admin");
        return app;
    }
}
