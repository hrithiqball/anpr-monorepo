using Newtonsoft.Json;

namespace MlffSiteService.DTOs;

public class ApiResponse<T> : ApiResponse
{
    public T Data { get; set; } = default!;
}

public class ApiResponse
{
    public int StatusCode { get; set; }

    public string Status { get; set; } = string.Empty;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Message { get; set; } = string.Empty;
}