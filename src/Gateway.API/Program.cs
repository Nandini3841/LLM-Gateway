using Gateway.Core;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.ConfigureHttpJsonOptions(o =>
    o.SerializerOptions.PropertyNamingPolicy = GatewayJson.NamingPolicy);
builder.Services.AddOllamaProvider(builder.Configuration);
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = GatewayJson.NamingPolicy);
builder.Services.AddHealthChecks();
var app = builder.Build();
app.UseExceptionHandler(errors => errors.Run(async context =>
{
    var ex = context.Features.Get<IExceptionHandlerFeature>()?.Error;
    var (status, type, message) = ex is ProviderException
        ? (StatusCodes.Status502BadGateway, "upstream_error", "The model provider is unavailable.")
        : (StatusCodes.Status500InternalServerError, "server_error", "Internal server error.");
    context.Response.StatusCode = status;
    await context.Response.WriteAsJsonAsync(
        new ErrorResponse { Error = new Error { Message = message, Type = type } },
        GatewayJson.Options);
}));
app.MapControllers();
app.MapHealthChecks("/healthz");
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Run();