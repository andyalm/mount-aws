using Amazon.CloudFront;
using Amazon.CloudFront.Model;
using static MountAws.PagingHelper;

namespace MountAws.Services.Cloudfront;

public static class CloudfrontApiExtensions
{
    public static IEnumerable<DistributionSummary> ListDistributions(this IAmazonCloudFront cloudfront)
    {
        return Paginate(nextToken =>
        {
            var response = cloudfront.ListDistributionsAsync(new ListDistributionsRequest
            {
                Marker = nextToken
            }).GetAwaiter().GetResult();

            return (response.DistributionList.Items, response.DistributionList.NextMarker);
        });
    }

    public static Distribution GetDistribution(this IAmazonCloudFront cloudfront, string id)
    {
        return cloudfront.GetDistributionAsync(new GetDistributionRequest(id)).GetAwaiter().GetResult().Distribution;
    }
}