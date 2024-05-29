using MlffWebApi.Interfaces.User;

namespace MlffWebApi.Interfaces.Speed;

public interface ISpeedRadar : ISpeedRadarLite, IUserCreatable, IUserModifiable
{
}

public interface ISpeedRadarLite
{
    string Id { get; set; }
    string Tag { get; set; }
    string DisplayName { get; set; }
}