namespace MountAws.Services.Cloudfront;

public class DistributionNotFoundException : ApplicationException
{
    public DistributionNotFoundException(string message) : base(message)
    {
        
    }
}