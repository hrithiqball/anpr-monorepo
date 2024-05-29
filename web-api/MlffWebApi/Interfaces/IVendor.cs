namespace MlffWebApi.Interfaces;

public interface IVendor
{
    public Guid Uid { get; set; }
    public string Name { get; set; }
    
    public string CreatedBy { get; set; }
    public DateTime DateCreated { get; set; }

    public string ModifiedBy { get; set; }
    public DateTime DateModified { get; set; }
}