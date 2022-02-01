using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.ServiceDiscovery;

public class ServiceDiscoveryRootHandler : PathHandler
{
    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "service-discovery",
            "Navigate cloudmap objects as a virtual filesystem");
    }
    
    public ServiceDiscoveryRootHandler(ItemPath path, IPathHandlerContext context) : base(path, context)
    {
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield return NamespacesHandler.CreateItem(Path);
    }
}