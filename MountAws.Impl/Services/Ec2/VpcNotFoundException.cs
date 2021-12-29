namespace MountAws.Api.Ec2;

public class VpcNotFoundException : ApplicationException
{
    public VpcNotFoundException(string vpcId) : base($"The vpc '{vpcId}' does not exist") {}
}