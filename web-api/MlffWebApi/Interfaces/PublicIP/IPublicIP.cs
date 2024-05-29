namespace MlffWebApi.Interfaces.PublicIP;


public interface IPublicIPLite
{
    public string PublicIPString { get; set; }
}

public interface IPublicIPRecognition : IPublicIPLite
{
    public Guid Uid { get; set; }
    string SiteId { get; set; }
    DateTime DateUpdate { get; set; }
}