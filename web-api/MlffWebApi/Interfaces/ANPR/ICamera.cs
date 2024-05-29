using MlffWebApi.Interfaces.User;

namespace MlffWebApi.Interfaces.ANPR;

public interface ICamera : ICameraLite, IUserCreatable, IUserModifiable
{
}

public interface ICameraLite
{
    string Id { get; set; }
    string Tag { get; set; }
    string DisplayName { get; set; }
}