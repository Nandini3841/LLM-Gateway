namespace Gateway.Core;

public class ErrorResponse
{
   public required Error Error { get; set; }
}

public class Error
{
    public required string Message { get; set; }
    public required string Type { get; set; }
    public string? Param { get; set; } = null;
    public string? Code { get; set; }
}