namespace MlffWebApi.Interfaces.User;

public interface IUserCreatable
{
    
    public string CreatedBy { get; set; }
    public DateTime DateCreated { get; set; }
}