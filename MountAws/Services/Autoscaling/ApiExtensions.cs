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
}