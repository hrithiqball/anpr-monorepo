using MlffWebApi.Hubs;
using MlffWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using MlffWebApi.Interfaces.Repository;
using MlffWebApi.Models.ANPR;
using MlffWebApi.Models.RFID;
using MlffWebApi.Models.Speed;

namespace MlffWebApi.Controllers;

public class TestController : Controller
{
    private readonly DetectionHub _detectionHub;
    private readonly IRepositoryFactory _repositoryFactory;

    public TestController(DetectionHub detectionHub, IRepositoryFactory repositoryFactory)
    {
        _detectionHub = detectionHub;
        _repositoryFactory = repositoryFactory;
    }

    [HttpGet("test/signal-r/matched")]
    public async Task<IActionResult> TestDetectionEvents(string siteId, string plateNumber, int? speed, string tagId,
        CancellationToken cancellationToken)
    {
        // get from watchlist
        var watchlist = await _repositoryFactory.WatchlistRepository.GetWatchlistByPlateNumberAsync(
            plateNumber, cancellationToken);

        if (!string.IsNullOrEmpty(tagId))
        {
            await _detectionHub.BroadcastRfidTagDetected(new RfidTagDetectionLite
            {
                SiteId = siteId,
                TagId = tagId,
                DetectionDate = DateTime.Now
            }, cancellationToken);
        }

        if (!string.IsNullOrEmpty(plateNumber))
        {
            await _detectionHub.BroadcastLicensePlateDetected(new LicensePlateRecognitionLite
            {
                SiteId = siteId,
                VehicleImagePath = @"/images/demo-vehicle.jpg",
                PlateImagePath = "/images/demo-plate.jpg",
                PlateNumber = plateNumber,
                IsInsideWatchlist = watchlist != null,
                DetectionDate = DateTime.Now
            }, cancellationToken);
        }

        if (speed.HasValue)
        {
            await _detectionHub.BroadcastSpeedDetected(new SpeedDetectionLite
            {
                SiteId = siteId,
                Speed = speed.Value,
                DetectionDate = DateTime.Now
            }, cancellationToken);
        }

        if (!string.IsNullOrEmpty(tagId) || !string.IsNullOrEmpty(plateNumber) || speed.HasValue)
        {
            var match = new DetectionMatchLite
            {
                SiteId = siteId,
                TagId = tagId,
                PlateNumber = plateNumber,
                VehicleImagePath = @"/images/demo-vehicle.jpg",
                PlateImagePath = "/images/demo-plate.jpg",
                Speed = speed,
                DateMatched = DateTime.Now
            };
            match.IsInsideWatchlist = watchlist != null;

            await _detectionHub.BroadcastDetectionMatched(match, cancellationToken);
        }

        return Ok();
    }
    //
    // private string GenerateRandomMalaysianCarPlate()
    // {
    //     // Create a list of all possible first letters for Malaysian car plates
    //     var firstLetters = new List<char>
    //     {
    //         'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U',
    //         'V', 'W', 'X', 'Y', 'Z'
    //     };
    //
    //     // Generate a random number between 0 and the number of possible first letters - 1
    //     var randomIndex = new Random().Next(0, firstLetters.Count - 1);
    //
    //     // Get the letter at the randomly generated index
    //     var firstLetter = firstLetters[randomIndex];
    //
    //     // Generate a random 4-digit number for the middle part of the car plate
    //     var middlePart = new Random().Next(1000, 9999).ToString();
    //
    //     // Create a list of all possible last letters for Malaysian car plates
    //     var lastLetters = new List<char>
    //     {
    //         'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U',
    //         'V', 'W', 'X', 'Y', 'Z'
    //     };
    //
    //     // Generate a random number between 0 and the number of possible last letters - 1
    //     var randomLastIndex = new Random().Next(0, lastLetters.Count - 1);
    //
    //     // Get the letter at the randomly generated index
    //     var lastLetter = lastLetters[randomLastIndex];
    //
    //     // Combine the first letter, middle part, and last letter to generate the car plate
    //     return $"{firstLetter}{middlePart}{lastLetter}";
    // }
}