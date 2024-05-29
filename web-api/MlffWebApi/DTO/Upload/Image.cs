namespace MlffWebApi.DTO.Upload;

public class UploadAnprRequestDto
{
    public string NumberPlate { get; set; }
    public string ImageId { get; set; }
    public IFormFile PlateImage { get; set; }
    public IFormFile VehicleImage { get; set; }
}

public class UploadAnprResponseDto
{
    public string PlateImageUrl { get; set; }
    public string VehicleImageUrl { get; set; }
}