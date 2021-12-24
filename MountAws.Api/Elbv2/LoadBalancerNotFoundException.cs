namespace MountAws.Api.Elbv2;

public class LoadBalancerNotFoundException : ApplicationException
{
    public LoadBalancerNotFoundException(string loadBalancerName) : base($"Load balancer '{loadBalancerName}' could not be found")
    {
        
    }
}