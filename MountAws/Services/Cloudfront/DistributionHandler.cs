using Amazon.CloudFront;
using MountAnything;

namespace MountAws.Services.Cloudfront;

public class DistributionHandler : PathHandler
{
    private readonly DistributionsHandler _parentHandler;

    public DistributionHandler(ItemPath path, IPathHandlerContext context, IAmazonCloudFront cloudfront) : base(path, context)
    {
        _parentHandler = new DistributionsHandler(path.Parent, context, cloudfront);
    }
    
    protected override IItem? GetItemImpl()
    {
        //we use parent handler instead of GetDistribution api because GetDistribution returns too different of an object from the DistributionSummary
        return _parentHandler.GetChildItems()
            .Cast<DistributionItem>()
            .SingleOrDefault(d => d.ItemName == ItemName || d.DomainName == ItemName);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield break;
    }
}