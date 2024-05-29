namespace MlffWebApi.Interfaces;

public interface IWebhook<TRequest>
{
    public Guid Uid { get; set; }
    public HttpMethodType Method { get; set; }
    public string Endpoint { get; set; }
    public Uri EndpointUri => new(Endpoint);
    public AuthType AuthType { get; set; }
    public TRequest Payload { get; set; }
    public IEnumerable<(string PropertyName, string Value)> Params { get; set; }
    
    public string CreatedBy { get; set; }
    public DateTime DateCreated { get; set; }

    public string ModifiedBy { get; set; }
    public DateTime DateModified { get; set; }
}

public interface IWebhook<TAuthProp, TRequest> : IWebhook<TRequest>
{
    public TAuthProp AuthProperties { get; set; }
}

public enum HttpMethodType
{
    GET,
    POST
}

public enum AuthType
{
    None,
    ApiKey,
    BearerToken,
    Basic,
    Digest
}