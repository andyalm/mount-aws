using Amazon.ServiceDiscovery;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.ServiceDiscovery;

public class NamespacesHandler : PathHandler
{
    private readonly IAmazonServiceDiscovery _serviceDiscovery;

    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "namespaces",
            "Navigate the cloudmap namespaces in the current account/region");
    }
    
    public NamespacesHandler(ItemPath path, IPathHandlerContext context, IAmazonServiceDiscovery serviceDiscovery) : base(path, context)
    {
        _serviceDiscovery = serviceDiscovery;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return _serviceDiscovery.ListNamespaces().Select(n => new NamespaceItem(Path, n));
    }
}