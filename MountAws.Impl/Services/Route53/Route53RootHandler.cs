using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Route53;

public class Route53RootHandler : PathHandler
{
    public static IItem CreateItem(string parentPath)
    {
        return new GenericContainerItem(parentPath, "route53",
            "Navigate Route53 objects as a virtual filesystem");
    }
    
    public Route53RootHandler(string path, IPathHandlerContext context) : base(path, context)
    {
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield return HostedZonesHandler.CreateItem(Path);
    }
}