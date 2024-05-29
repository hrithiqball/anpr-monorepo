using MlffWebApi.Database.DbContexts;
using MlffWebApi.DTO.Watchlist;
using MlffWebApi.Interfaces.ANPR;
using Newtonsoft.Json;

namespace MlffWebApi.Models;

public class Watchlist : IWatchlist
{
    public Guid Uid { get; set; }

    [JsonIgnore]
    public WatchlistMonitorOptions MonitorOption { get; set; }

    [JsonProperty("monitorOption")]
    public string MonitorOptionStr => MonitorOption.ToString();

    public string Value { get; set; }
    public string CreatedBy { get; set; }
    public DateTime DateCreated { get; set; }
    public string ModifiedBy { get; set; }
    public DateTime DateModified { get; set; }
    public string Remarks { get; set; }
    public string TagColor { get; set; }
}

public static class WatchlistExtension
{
    public static watchlist ToDatabaseModel(this AddWatchlistRequestDto.Watchlist tmp, string username)
    {
        return new watchlist
        {
            uid = Guid.NewGuid(),
            monitor_option = (int)tmp.MonitorOption,
            value = tmp.Value,
            created_by = username,
            date_created = DateTime.Now,
            modified_by = username,
            date_modified = DateTime.Now,
            remarks = tmp.Remarks,
            tag_color = tmp.TagColor
        };
    }

    public static watchlist ToDatabaseModel(this IWatchlist tmp)
    {
        return new watchlist
        {
            uid = tmp.Uid,
            monitor_option = (int)tmp.MonitorOption,
            value = tmp.Value ??
                    throw new NullReferenceException($"Trigger value cannot be null. " +
                                                     $"Please specify the {tmp.MonitorOption} as trigger value."),
            created_by = tmp.CreatedBy,
            date_created = tmp.DateCreated,
            modified_by = tmp.ModifiedBy,
            date_modified = tmp.DateModified,
        };
    }

    public static IWatchlist ToBusinessModel(this watchlist tmp)
    {
        return new Watchlist
        {
            Uid = tmp.uid,
            MonitorOption = (WatchlistMonitorOptions)tmp.monitor_option,
            Value = tmp.value,
            CreatedBy = tmp.created_by,
            DateCreated = tmp.date_created,
            ModifiedBy = tmp.modified_by,
            DateModified = tmp.date_modified,
            Remarks = tmp.remarks,
            TagColor = tmp.tag_color
        };
    }
}