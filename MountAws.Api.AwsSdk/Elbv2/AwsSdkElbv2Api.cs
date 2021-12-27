using System.Management.Automation;
using System.Security.AccessControl;
using Amazon.ElasticLoadBalancingV2;
using Amazon.ElasticLoadBalancingV2.Model;
using MountAnything;
using MountAws.Api.Elbv2;
using DescribeLoadBalancersResponse = MountAws.Api.Elbv2.DescribeLoadBalancersResponse;
using DescribeTargetGroupsRequest = MountAws.Api.Elbv2.DescribeTargetGroupsRequest;
using LoadBalancerNotFoundException = Amazon.ElasticLoadBalancingV2.Model.LoadBalancerNotFoundException;
using TargetGroupNotFoundException = Amazon.ElasticLoadBalancingV2.Model.TargetGroupNotFoundException;

namespace MountAws.Api.AwsSdk.Elbv2;

public class AwsSdkElbv2Api : IElbv2Api
{
    private readonly IAmazonElasticLoadBalancingV2 _elbv2;

    public AwsSdkElbv2Api(IAmazonElasticLoadBalancingV2 elbv2)
    {
        _elbv2 = elbv2;
    }

    public PSObject DescribeLoadBalancer(string loadBalancerName)
    {
        try
        {
            return _elbv2.DescribeLoadBalancersAsync(new DescribeLoadBalancersRequest
            {
                Names = new List<string> { loadBalancerName }
            }).GetAwaiter().GetResult().LoadBalancers.Single().ToPSObject();
        }
        catch (LoadBalancerNotFoundException)
        {
            throw new Api.Elbv2.LoadBalancerNotFoundException(loadBalancerName);
        }
    }

    public DescribeLoadBalancersResponse DescribeLoadBalancers(string? nextToken = null)
    {
        var response = _elbv2.DescribeLoadBalancersAsync(new DescribeLoadBalancersRequest
        {
            Marker = nextToken
        }).GetAwaiter().GetResult();

        return new DescribeLoadBalancersResponse(
            response.LoadBalancers.ToPSObjects().ToArray(),
            response.NextMarker
        );
    }

    public IEnumerable<PSObject> DescribeListeners(string loadBalancerArn)
    {
        return _elbv2.DescribeListenersAsync(new DescribeListenersRequest
        {
            LoadBalancerArn = loadBalancerArn
        }).GetAwaiter().GetResult().Listeners.ToPSObjects();
    }

    public (IEnumerable<PSObject> TargetGroups, string NextToken) DescribeTargetGroups(string? nextToken)
    {
        var response = _elbv2
            .DescribeTargetGroupsAsync(new Amazon.ElasticLoadBalancingV2.Model.DescribeTargetGroupsRequest
            {
                Marker = nextToken,
                PageSize = 100
            })
            .GetAwaiter().GetResult();

        return (response.TargetGroups.ToPSObjects(), response.NextMarker);
    }

    public IEnumerable<PSObject> DescribeTargetGroups(DescribeTargetGroupsRequest request)
    {
        var sdkRequest = new Amazon.ElasticLoadBalancingV2.Model.DescribeTargetGroupsRequest
        {
            Names = request.Names,
            TargetGroupArns = request.Arns
        };

        try
        {
            return _elbv2.DescribeTargetGroupsAsync(sdkRequest)
                .GetAwaiter()
                .GetResult()
                .TargetGroups
                .ToPSObjects();
        }
        catch (TargetGroupNotFoundException ex)
        {
            throw new Api.Elbv2.TargetGroupNotFoundException(ex.Message);
        }
    }

    public IEnumerable<PSObject> DescribeRules(string listenerArn)
    {
        return _elbv2.DescribeRulesAsync(new DescribeRulesRequest
        {
            ListenerArn = listenerArn
        }).GetAwaiter().GetResult().Rules.ToPSObjects();
    }

    public IEnumerable<PSObject> DescribeTargetHealth(string targetGroupArn)
    {
        return _elbv2.DescribeTargetHealthAsync(new DescribeTargetHealthRequest
        {
            TargetGroupArn = targetGroupArn
        }).GetAwaiter().GetResult().TargetHealthDescriptions.ToPSObjects();
    }
}