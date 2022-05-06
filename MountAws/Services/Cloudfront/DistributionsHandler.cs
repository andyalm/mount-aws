using Amazon.CloudFront;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Cloudfront;

public class DistributionsHandler : PathHandler
{
    private readonly IAmazonCloudFront _cloudfront;

    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "distributions",
            "Navigate cloudfront distributions as a virtual filesystem");
    }
    
    public DistributionsHandler(ItemPath path, IPathHandlerContext context, IAmazonCloudFront cloudfront) : base(path, context)
    {
        _cloudfront = cloudfront;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return _cloudfront.ListDistributions()
            .Select(d => new DistributionItem(Path, d, LinkGenerator));
    }
}