using System.Text.Json;

namespace Gateway.Core;

/// <summary>
/// The single source of truth for JSON naming. The API and the tests both use this,
/// so the wire format (OpenAI-style snake_case) can't drift between them.
/// </summary>
public static class GatewayJson
{
    public static readonly JsonNamingPolicy NamingPolicy = JsonNamingPolicy.SnakeCaseLower;

    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = NamingPolicy
    };
}
