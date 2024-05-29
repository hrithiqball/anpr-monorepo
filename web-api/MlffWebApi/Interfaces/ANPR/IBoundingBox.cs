namespace MlffWebApi.Interfaces.ANPR;

public interface IBoundingBox
{
    int? Top { get; set; }
    int? Left { get; set; }
    int? Width { get; set; }
    int? Height { get; set; }
}