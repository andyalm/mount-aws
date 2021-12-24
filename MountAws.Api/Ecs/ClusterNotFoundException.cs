namespace MountAws.Api.Ecs;

public class ClusterNotFoundException : ApplicationException
{
    public ClusterNotFoundException(string clusterName) : base($"The cluster '{clusterName}' could not be found")
    {
        
    }
}