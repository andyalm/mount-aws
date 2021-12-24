namespace MountAws.Api.Ecs;

public class ContainerInstanceNotFoundException : ApplicationException
{
    public ContainerInstanceNotFoundException(string cluster, string containerInstanceId) 
        : base($"Container instance '{containerInstanceId}' could not be found in cluster '{cluster}'")
    {
        
    }
}