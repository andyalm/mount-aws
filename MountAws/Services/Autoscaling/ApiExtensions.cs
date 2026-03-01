using Amazon.ApplicationAutoScaling;
using Amazon.ApplicationAutoScaling.Model;
using static MountAws.PagingHelper;

namespace MountAws.Services.Autoscaling;

public static class ApiExtensions
{
    public static IEnumerable<ScalableTarget> DescribeScalableTargets(this IAmazonApplicationAutoScaling autoScaling, ServiceNamespace serviceNamespace)
    {
        return Paginate(nextToken =>
        {
            var response = autoScaling.DescribeScalableTargetsAsync(new DescribeScalableTargetsRequest
            {
                ServiceNamespace = serviceNamespace,
                NextToken = nextToken
            }).GetAwaiter().GetResult();

            return (response.ScalableTargets, response.NextToken);
        });
    }

    public static IEnumerable<ScalingPolicy> DescribeScalingPolicies(this IAmazonApplicationAutoScaling autoScaling, ServiceNamespace serviceNamespace, string resourceId)
    {
        return Paginate(nextToken =>
        {
            var response = autoScaling.DescribeScalingPoliciesAsync(new DescribeScalingPoliciesRequest
            {
                ServiceNamespace = serviceNamespace,
                ResourceId = resourceId,
                NextToken = nextToken
            }).GetAwaiter().GetResult();

            return (response.ScalingPolicies, response.NextToken);
        });
    }

    public static IEnumerable<ScalingActivity> DescribeScalingActivities(this IAmazonApplicationAutoScaling autoScaling, ServiceNamespace serviceNamespace, string resourceId)
    {
        return Paginate(nextToken =>
        {
            var response = autoScaling.DescribeScalingActivitiesAsync(new DescribeScalingActivitiesRequest
            {
                ServiceNamespace = serviceNamespace,
                ResourceId = resourceId,
                NextToken = nextToken
            }).GetAwaiter().GetResult();

            return (response.ScalingActivities, response.NextToken);
        });
    }

    public static IEnumerable<ScheduledAction> DescribeScheduledActions(this IAmazonApplicationAutoScaling autoScaling, ServiceNamespace serviceNamespace, string resourceId)
    {
        return Paginate(nextToken =>
        {
            var response = autoScaling.DescribeScheduledActionsAsync(new DescribeScheduledActionsRequest
            {
                ServiceNamespace = serviceNamespace,
                ResourceId = resourceId,
                NextToken = nextToken
            }).GetAwaiter().GetResult();

            return (response.ScheduledActions, response.NextToken);
        });
    }
}
