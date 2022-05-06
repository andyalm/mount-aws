using Amazon;
using Amazon.WAFV2;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Wafv2;

public class WebAclsHandler : PathHandler
{
    private readonly IAmazonWAFV2 _wafv2;
    private readonly Scope _scope;

    public static IItem CreateItem(ItemPath parentPath, Scope scope)
    {
        return scope.ToString() switch
        {
            nameof(Scope.CLOUDFRONT) => new GenericContainerItem(parentPath, "cloudfront-web-acls",
                "Navigate cloudfront web acls as a virtual filesystem"),
            nameof(Scope.REGIONAL) => new GenericContainerItem(parentPath, "regional-web-acls",
                "Navigate regional web acls as a virtual filesystem"),
            _ => throw new ArgumentOutOfRangeException(nameof(scope))
        };
    }
    
    public WebAclsHandler(ItemPath path, IPathHandlerContext context, IAmazonWAFV2 wafv2, Scope scope) : base(path, context)
    {
        _wafv2 = wafv2;
        _scope = scope;
    }

    protected override IItem? GetItemImpl()
    {
        if (_scope == Scope.CLOUDFRONT && _wafv2.Config.RegionEndpoint.SystemName != RegionEndpoint.USEast1.SystemName)
        {
            return null;
        }
        return CreateItem(ParentPath, _scope);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return _wafv2.ListWebAcls(_scope).Select(a => new WebAclItem(Path, a));
    }
}