using System.Net;
using Newtonsoft.Json;

namespace MlffWebApi.DTO;

public class ApiResponse
{
    public ApiResponse(int statusCode, string message = null)
    {
        StatusCode = (HttpStatusCode)statusCode;
        Message = message;
    }

    public ApiResponse(HttpStatusCode statusCode, string message = null)
    {
        StatusCode = statusCode;
        Message = message;
    }

    [JsonProperty(Order = 0)]
    public HttpStatusCode StatusCode { get; }

    [JsonProperty(Order = 1)]
    public string Status
    {
        get
        {
            switch (StatusCode)
            {
                case HttpStatusCode.InternalServerError: return "Internal Server Error";
                default: return StatusCode.ToString();
            }
        }
    }

    [JsonProperty(Order = 2, NullValueHandling = NullValueHandling.Ignore)]
    public string Message { get; }
}

public class ApiResponse<T> : ApiResponse
{
    public ApiResponse(int statusCode, T data) : base(statusCode)
    {
        Data = data;
    }

    public ApiResponse(HttpStatusCode statusCode, T data) : base(statusCode)
    {
        Data = data;
    }

    public ApiResponse(int statusCode, string message, T data) : base(statusCode, message)
    {
        Data = data;
    }

    public ApiResponse(HttpStatusCode statusCode, string message, T data) : base(statusCode, message)
    {
        Data = data;
    }

    [JsonProperty(Order = 10)]
    public T Data { get; }
}