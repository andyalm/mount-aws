namespace MountAws.Api.Ec2;

public class SubnetNotFoundException : ApplicationException
{
    public SubnetNotFoundException(string subnetId) : base($"The subnet '{subnetId}' does not exist")
    {
        
    }
}