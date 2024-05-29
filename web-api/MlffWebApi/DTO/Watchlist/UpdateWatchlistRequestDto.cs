using MlffWebApi.Interfaces;
using MlffWebApi.Models;

namespace MlffWebApi.DTO.Watchlist;

public class UpdateWatchlistRequestDto
{
    public string Value { get; set; }
    public string Remarks { get; set; }
    public string TagColor { get; set; }
}