using Amazon.ElasticLoadBalancingV2;
using Amazon.ElasticLoadBalancingV2.Model;

using static MountAws.PagingHelper;
using Action = Amazon.ElasticLoadBalancingV2.Model.Action;

namespace MountAws.Services.Elbv2;

public static class Elbv2ApiExtensions
{
    public static LoadBalancer DescribeLoadBalancer(this IAmazonElasticLoadBalancingV2 elbv2, string loadBalancerName)
    {
        return elbv2.DescribeLoadBalancersAsync(new DescribeLoadBalancersRequest
        {
            Names = new List<string> { loadBalancerName }
        }).GetAwaiter().GetResult().LoadBalancers.Single();
    }

    public static IEnumerable<LoadBalancer> DescribeLoadBalancers(this IAmazonElasticLoadBalancingV2 elbv2)
    {
        return Paginate(nextToken =>
        {
            var response = elbv2.DescribeLoadBalancersAsync(new DescribeLoadBalancersRequest
            {
                Marker = nextToken
            }).GetAwaiter().GetResult();

            return (response.LoadBalancers, response.NextMarker);
        });
    }

    public static IEnumerable<Listener> DescribeListeners(this IAmazonElasticLoadBalancingV2 elbv2, string loadBalancerArn)
    {
        return elbv2.DescribeListenersAsync(new DescribeListenersRequest
        {
            LoadBalancerArn = loadBalancerArn
        }).GetAwaiter().GetResult().Listeners;
    }

    public static IEnumerable<TargetGroup> DescribeTargetGroups(this IAmazonElasticLoadBalancingV2 elbv2)
    {
        return Paginate(nextToken =>
        {
            var response = elbv2
                .DescribeTargetGroupsAsync(new DescribeTargetGroupsRequest
                {
                    Marker = nextToken,
                    PageSize = 100
                })
                .GetAwaiter().GetResult();

            return (response.TargetGroups, response.NextMarker);
        });
    }

    public static IEnumerable<TargetGroup> DescribeTargetGroups(this IAmazonElasticLoadBalancingV2 elbv2, DescribeTargetGroupsRequest request)
    {
        return elbv2.DescribeTargetGroupsAsync(request)
            .GetAwaiter()
            .GetResult()
            .TargetGroups;
    }

    public static void DeleteTargetGroup(this IAmazonElasticLoadBalancingV2 elbv2, string targetGroupArn)
    {
        elbv2.DeleteTargetGroupAsync(new DeleteTargetGroupRequest
        {
            TargetGroupArn = targetGroupArn
        }).GetAwaiter().GetResult();
    }

    public static IEnumerable<Rule> DescribeRules(this IAmazonElasticLoadBalancingV2 elbv2, string listenerArn)
    {
        return elbv2.DescribeRulesAsync(new DescribeRulesRequest
        {
            ListenerArn = listenerArn
        }).GetAwaiter().GetResult().Rules;
    }

    public static void DeleteRule(this IAmazonElasticLoadBalancingV2 elbv2, string ruleArn)
    {
        elbv2.DeleteRuleAsync(new DeleteRuleRequest
        {
            RuleArn = ruleArn
        }).GetAwaiter().GetResult();
    }

    public static IEnumerable<TargetHealthDescription> DescribeTargetHealth(this IAmazonElasticLoadBalancingV2 elbv2, string targetGroupArn)
    {
        return elbv2.DescribeTargetHealthAsync(new DescribeTargetHealthRequest
        {
            TargetGroupArn = targetGroupArn
        }).GetAwaiter().GetResult().TargetHealthDescriptions;
    }

    public static IEnumerable<Certificate> DescribeListenerCertificates(this IAmazonElasticLoadBalancingV2 elbv2, string listenerArn)
    {
        return Paginate(nextToken =>
        {
            var response = elbv2.DescribeListenerCertificatesAsync(new DescribeListenerCertificatesRequest
            {
                ListenerArn = listenerArn,
                Marker = nextToken
            }).GetAwaiter().GetResult();

            return (response.Certificates, response.NextMarker);
        });
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
        return elbv2.DescribeTargetGroups(request).Single();
    }

    public static string TargetGroupName(this TargetGroupTuple targetGroupTuple)
    {
        return Elbv2.Elbv2ArnDecoder.TargetGroupName(targetGroupTuple.TargetGroupArn);
    }

    public static string? TargetGroupArn(this Action action)
    {
        return string.IsNullOrEmpty(action.TargetGroupArn)
            ? action.ForwardConfig.TargetGroups.Single().TargetGroupArn
            : action.TargetGroupArn;
    }

    public static string? TargetGroupName(this Action action)
    {
        return TargetGroupNameFromArn(action.TargetGroupArn());
    }

    private static string? TargetGroupNameFromArn(string? targetGroupArn)
    {
        return targetGroupArn?.Split("/")[^2];
    }
}