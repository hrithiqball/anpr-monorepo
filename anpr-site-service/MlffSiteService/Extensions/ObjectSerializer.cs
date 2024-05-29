using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MlffSiteService.Extensions;

public static class ObjectSerializer
{
    public static string Serialize(this object obj, bool isIndented = true)
    {
        return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() },
            Formatting = isIndented ? Formatting.Indented: Formatting.None
        });
    }

    public static T? Deserialize<T>(this string tmp) where T : class
    {
        return JsonConvert.DeserializeObject<T>(tmp);
    }
}