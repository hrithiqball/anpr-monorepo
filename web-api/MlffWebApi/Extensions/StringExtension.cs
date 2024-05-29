namespace MlffWebApi.Extensions;

public static class StringExtension
{
    public static bool Contains(this string text1, string text2, bool isCaseSensitive)
    {
        if (text1 is null)
        {
            throw new NullReferenceException(nameof(text1));
        }

        if (isCaseSensitive && text2 is null)
        {
            throw new NullReferenceException(nameof(text2));
        }

        return isCaseSensitive ? text1.ToLower().Contains(text2.ToLower()) : text1.Contains(text2);
    }
}