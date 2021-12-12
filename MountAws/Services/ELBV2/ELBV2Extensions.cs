using Amazon.ElasticLoadBalancingV2;
using Amazon.ElasticLoadBalancingV2.Model;

namespace MountAws.Services.ELBV2;

public static class ELBV2Extensions
{
    public static LoadBalancer? GetLoadBalancer(this IAmazonElasticLoadBalancingV2 elbv2, string loadBalancerName)
    {
        var request = new DescribeLoadBalancersRequest
        {
            Names = new List<string> { loadBalancerName }
        };
        var response = elbv2.DescribeLoadBalancersAsync(request).GetAwaiter().GetResult();

        if (response.LoadBalancers.Count == 1)
        {
            return response.LoadBalancers.Single();
        }

        return null;
    }

    public static Listener[] GetListeners(this IAmazonElasticLoadBalancingV2 elbv2, string loadBalancerArn)
    {
        var response = elbv2.DescribeListenersAsync(new DescribeListenersRequest
        {
            LoadBalancerArn = loadBalancerArn
        });

        return response.GetAwaiter().GetResult().Listeners.ToArray();
    }

    public static TargetGroup GetTargetGroup(this IAmazonElasticLoadBalancingV2 elbv2, string targetGroupNameOrArn)
    {
        var request = new DescribeTargetGroupsRequest();
        if (targetGroupNameOrArn.StartsWith("arn:"))
        {
            request.TargetGroupArns.Add(targetGroupNameOrArn);
        }
        else
        {
            request.Names.Add(targetGroupNameOrArn);
        }
        var response = elbv2.DescribeTargetGroupsAsync(request).GetAwaiter().GetResult();

        return response.TargetGroups.Single();
    }

    public static string TargetGroupName(string targetGroupArn)
    {
        return targetGroupArn.Split("/")[^2];
    }

    public static string Description(this RuleCondition condition)
    {
        switch (condition.Field)
        {
            case "http-header":
                return
                $"{condition.Field} {condition.HttpHeaderConfig.HttpHeaderName} matches ({string.Join(",", condition.HttpHeaderConfig.Values)})";
            default:
                return $"{condition.Field} matches ({string.Join(",", condition.Values)})";
        }
    }
}