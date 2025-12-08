using System.Text.Json;
using System.Text.Json.Serialization;
using Flurl.Http;
using Flurl.Http.Configuration;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;

namespace Integration.Tests.Utilities;

public class ApiClient : FlurlClient
{
    private readonly WebApplicationFactory<Program> _factory;

    public ApiClient(HttpClient httpClient, WebApplicationFactory<Program> factory) : base(httpClient)
    {
        _factory = factory;
        Settings.JsonSerializer = new DefaultJsonSerializer(JsonOptions.Default);
        this.OnError(async call =>
            {
                if (call.Response is { StatusCode: < 200 or >= 300 })
                {
                    var responseBody = await call.Response.GetStringAsync();
                    var message = call.Exception.Message;

                    Exception exception = call.Response.StatusCode switch
                    {
                        400 => new HttpBadRequestException(responseBody, message),
                        404 => new HttpNotFoundException(responseBody, message),
                        _ => new HttpInternalServerErrorException(responseBody, message)
                    };

                    throw exception;
                }
            }
        );
    }

    public HubConnection CreateLiveUpdatesHubConnection() =>
        new HubConnectionBuilder()
            .WithUrl($"{_factory.Server.BaseAddress}ws/updates",
                options => { options.HttpMessageHandlerFactory = _ => _factory.Server.CreateHandler(); })
            .WithAutomaticReconnect()
            .Build();

    private static class JsonOptions
    {
        public static readonly JsonSerializerOptions Default = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // match camelCase JSON
            PropertyNameCaseInsensitive = true
        };

        static JsonOptions()
        {
            Default.Converters.Add(new JsonStringEnumConverter()); // enums as strings
        }
    }
}

public abstract class HttpException : Exception
{
    internal string ErrorResponse { get; set; }

    protected HttpException(string errorResponse, string message) : base(message + "\n" + errorResponse)
    {
        ErrorResponse = errorResponse;
    }
}

internal class HttpBadRequestException : HttpException
{
    public HttpBadRequestException(string errorResponse, string message) : base(errorResponse, message)
    {
    }
}

internal class HttpNotFoundException : HttpException
{
    public HttpNotFoundException(string errorResponse, string message) : base(errorResponse, message)
    {
    }
}

internal class HttpInternalServerErrorException : HttpException
{
    public HttpInternalServerErrorException(string errorResponse, string message) : base(errorResponse, message)
    {
    }
}