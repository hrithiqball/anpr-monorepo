namespace MlffWebApi.Extensions;

public static class NumberExtension
{
    public static double ToDouble(this decimal value) => (double)value;
    
    public static double ToDouble(this int value) => (double)value;

    public static int ToInt(this double value) => (int)value;

    public static decimal ToDecimal(this double value) => (decimal)value;
}