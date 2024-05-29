namespace MlffWebApi.Interfaces;

public interface IThirdPartyIntegrator
{
    public string Token { get; set; }
    public DateTime ExpiryDate { get; set; }
    public Guid VendorUid { get; set; }
    public IVendor Vendor { get; set; }
    
    public string CreatedBy { get; set; }
    public DateTime DateCreated { get; set; }

    public string ModifiedBy { get; set; }
    public DateTime DateModified { get; set; }
}