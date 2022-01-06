using Amazon;
using Amazon.WAFV2;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Wafv2;

public class Wafv2RootHandler : PathHandler
{
    private readonly RegionEndpoint _region;

    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "wafv2",
            "Navigate WAF acls as a virtual filesystem");
    }
    
    public Wafv2RootHandler(ItemPath path, IPathHandlerContext context, RegionEndpoint region) : base(path, context)
    {
        _region = region;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        if (_region.SystemName == RegionEndpoint.USEast1.SystemName)
        {
            // cloudfront items only available from us-east-1
            yield return WebAclsHandler.CreateItem(Path, Scope.CLOUDFRONT);
        }
        yield return WebAclsHandler.CreateItem(Path, Scope.REGIONAL);
    }
}