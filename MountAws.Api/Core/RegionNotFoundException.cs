namespace MountAws.Api;

public class RegionNotFoundException : ApplicationException
{
    public RegionNotFoundException(string regionName) : base($"The region '{regionName}' does not exist")
    {
        
    }
}