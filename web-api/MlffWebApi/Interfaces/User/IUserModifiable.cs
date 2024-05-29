namespace MlffWebApi.Interfaces.User;

public interface IUserModifiable
{
    public string ModifiedBy { get; set; }
    public DateTime DateModified { get; set; }
}