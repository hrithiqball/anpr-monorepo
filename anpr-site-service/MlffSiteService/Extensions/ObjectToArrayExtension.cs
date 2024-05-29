namespace MlffSiteService.Extensions;

public static class ObjectToArrayExtension
{
    public static T[] ToArray<T>(this T obj)
    {
        return new[] { obj };
    }
}