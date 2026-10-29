using Gateway.API.Endpoints;
using Gateway.Core.Chat;
using Gateway.Providers;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();
builder.Services.AddOllamaProvider(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler(errors => errors.Run(async context =>
{
    var ex = context.Features.Get<IExceptionHandlerFeature>()?.Error;
    var (status, title) = ex switch
    {
        ProviderException => (StatusCodes.Status502BadGateway, "Upstream provider error."),
        BadHttpRequestException => (StatusCodes.Status400BadRequest, "Invalid request."),
        _ => (StatusCodes.Status500InternalServerError, "Internal server error.")
    };
    context.Response.StatusCode = status;
    await context.Response.WriteAsJsonAsync(new { title, status });
}));

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapHealthChecks("/healthz");
app.MapChatEndpoints();
app.MapAdminEndpoints();

app.Run();
