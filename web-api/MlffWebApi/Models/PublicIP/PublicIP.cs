using MlffWebApi.Database.DbContexts;
using MlffWebApi.Interfaces.PublicIP;

namespace MlffWebApi.Models.PublicIP;

public class PublicIPLite : IPublicIPLite
{
    public string PublicIPString { get; set; }
}

public class PublicIPRecognition : PublicIPLite, IPublicIPRecognition
{
    public DateTime DateUpdate { get; set; }
    public string SiteId { get; set; }
    public Guid Uid { get; set; }
}

public static class PublicIPRecognitionExtension
{
    public static IPublicIPRecognition ToBusinessModel(this public_ip pr)
    {
        return new PublicIPRecognition
        {
            Uid = pr.uid,
            PublicIPString = pr.ip_address,
            DateUpdate = pr.date_update,
            SiteId = pr.site_id,
        };
    }

    public static IPublicIPRecognition ToLite(this IPublicIPRecognition pr)
    {
        return pr;
    }

    public static public_ip ToDatabaseModel(this IPublicIPRecognition pr)
    {
        return new public_ip
        {
            uid = Guid.NewGuid(),
            ip_address = pr.PublicIPString,
            date_update = pr.DateUpdate,
            site_id = pr.SiteId,
        };
    }
}