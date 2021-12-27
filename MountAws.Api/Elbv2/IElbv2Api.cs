using System.Management.Automation;

namespace MountAws.Api.Elbv2;

public interface IElbv2Api
{
    PSObject DescribeLoadBalancer(string loadBalancerName);
    
    DescribeLoadBalancersResponse DescribeLoadBalancers(string? nextToken = null);

    IEnumerable<PSObject> DescribeListeners(string loadBalancerArn);

    IEnumerable<PSObject> DescribeTargetGroups(DescribeTargetGroupsRequest request);
    IEnumerable<PSObject> DescribeRules(string listenerArn);

    IEnumerable<PSObject> DescribeTargetHealth(string targetGroupArn);
    (IEnumerable<PSObject> TargetGroups, string NextToken) DescribeTargetGroups(string? nextToken);
    void DeleteTargetGroup(string targetGroupArn);
    void DeleteRule(string ruleArn);
}

public class DescribeLoadBalancersResponse
{
    public DescribeLoadBalancersResponse(PSObject[] loadBalancers, string nextToken)
    {
        LoadBalancers = loadBalancers;
        NextToken = nextToken;
    }

    public PSObject[] LoadBalancers { get; }
    public string NextToken { get; }
}