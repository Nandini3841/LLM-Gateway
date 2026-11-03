namespace Gateway.Core.Pipeline;

public class PipelineContext
{
    public required InfoRequest InfoRequest { get; set; }
    public InfoResponse InfoResponse { get; set; } = new InfoResponse();
}