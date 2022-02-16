using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Cloudfront;

public class CloudfrontRootHandler : PathHandler
{
    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "cloudfront",
            "Navigate cloudfront objects in a virtual filesystem");
    }
    
    public CloudfrontRootHandler(ItemPath path, IPathHandlerContext context) : base(path, context)
    {
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield return DistributionsHandler.CreateItem(Path);
    }
}