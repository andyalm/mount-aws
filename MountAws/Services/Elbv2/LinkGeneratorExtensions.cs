using MountAnything;

namespace MountAws.Services.Elbv2;

public static class LinkGeneratorExtensions
{
    public static ItemPath TargetGroup(this LinkGenerator linkGenerator, string targetGroupNameOrArn)
    {
        var targetGroupName = targetGroupNameOrArn.StartsWith("arn:") ? Elbv2ArnDecoder.TargetGroupName(targetGroupNameOrArn) : targetGroupNameOrArn;

        return linkGenerator.Elbv2ServicePath().Combine("target-groups", targetGroupName);
    }

    public static ItemPath LoadBalancer(this LinkGenerator linkGenerator, string loadBalancerNameOrArn)
    {
        var loadBalancerName = loadBalancerNameOrArn.StartsWith("arn:")
            ? Elbv2ArnDecoder.LoadBalancerName(loadBalancerNameOrArn)
            : loadBalancerNameOrArn;

        return linkGenerator.Elbv2ServicePath().Combine("load-balancers", loadBalancerName);
    }
    
    private static ItemPath Elbv2ServicePath(this LinkGenerator linkGenerator)
    {
        return linkGenerator.ConstructPath(2, "elbv2");
    }
}