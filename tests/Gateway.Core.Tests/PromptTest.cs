using Gateway.Core;
using System.Text.Json;
namespace tests;

public class PromptTest
{
    [Fact]
    public void Request_Deserializes_Properly()
    {
        var json = """
                   {
                     "model": "fast",
                     "messages": [
                       { "role": "system", "content": "You are helpful." },
                       { "role": "user", "content": "Hi" }
                     ],
                     "stream": false,
                     "temperature": 0.7,
                     "max_tokens": 200
                   }
                   """;
        
        var request = JsonSerializer.Deserialize<PromptRequest>(json, GatewayJson.Options);
        Assert.NotNull(request);
        Assert.Equal("fast", request.Model);
        Assert.Equal(2, request.Messages.Count);
        Assert.Equal(200, request.MaxTokens);
        Assert.Equal(0.7, request.Temperature);
    }
    
    [Fact]
    public void Response_Deserializes_Properly()
    {
        var json = """
                   {
                     "id": "chatcmpl-9f3a1c2b7d4e",
                     "object": "chat.completion",
                     "created": 1730246400,
                     "model": "llama3.2",
                     "choices": [
                       {
                         "index": 0,
                         "message": {
                           "role": "assistant",
                           "content": "Hello! How can I help you today?"
                         },
                         "finish_reason": "stop"
                       }
                     ],
                     "usage": {
                       "prompt_tokens": 12,
                       "completion_tokens": 9,
                       "total_tokens": 21
                     }
                   }
                   """;
        
        var response = JsonSerializer.Deserialize<PromptResponse>(json, GatewayJson.Options);
        Assert.NotNull(response);
        Assert.Equal("chatcmpl-9f3a1c2b7d4e", response.Id);
        Assert.Equal("chat.completion", response.Object);
        Assert.Equal(1730246400L, response.Created);
        Assert.Equal("llama3.2", response.Model);
        var choice = Assert.Single(response.Choices);
        Assert.Equal(0, choice.Index);
        Assert.Equal("stop", choice.FinishReason);
        Assert.Equal("assistant", choice.Message.Role);
        Assert.Equal("Hello! How can I help you today?", choice.Message.Content);
        Assert.Equal(12, response.Usage.PromptTokens);
        Assert.Equal(9, response.Usage.CompletionTokens);
        Assert.Equal(21, response.Usage.TotalTokens);
    }
}

