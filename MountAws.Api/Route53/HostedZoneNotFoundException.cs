namespace MountAws.Api.Route53;

public class HostedZoneNotFoundException : ApplicationException
{
    public HostedZoneNotFoundException(string id) : base($"Hosted zone '{id}' does not exist")
    {
        
    }
}