using MlffWebApi.Interfaces.User;

namespace MlffWebApi.Interfaces.RFID;

public interface IRfidReader : IRfidReaderLite, IUserCreatable, IUserModifiable
{
}

public interface IRfidReaderLite
{
    string Id { get; set; }
    string DisplayName { get; set; }
    string Tag { get; set; }
}