using MlffSiteService.Interface;

namespace MlffSiteService.Models;

public class BoundingBox : IBoundingBox
{
    public int Top { get; set; }

    public int Left { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }
}