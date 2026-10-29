namespace Gateway.Providers.Ollama;

public sealed class OllamaOptions
{
    public const string SectionName = "Providers:Ollama";

    public Uri BaseUrl { get; set; } = new("http://localhost:11434");
}
