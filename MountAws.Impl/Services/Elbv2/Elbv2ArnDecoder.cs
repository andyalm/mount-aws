namespace MountAws.Services.Elbv2;

public static class Elbv2ArnDecoder
{
    public static string TargetGroupName(string targetGroupArn)
    {
        return targetGroupArn.Split("/")[^2];
    }

    public static string LoadBalancerName(string loadBalancerArn)
    {
        return loadBalancerArn.Split("/")[^2];
    }
}