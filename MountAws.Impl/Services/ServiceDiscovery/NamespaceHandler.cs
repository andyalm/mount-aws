using Amazon.ServiceDiscovery;
using MountAnything;

namespace MountAws.Services.ServiceDiscovery;

public class NamespaceHandler : PathHandler
{
    private readonly IAmazonServiceDiscovery _serviceDiscovery;

    public NamespaceHandler(ItemPath path, IPathHandlerContext context, IAmazonServiceDiscovery serviceDiscovery) : base(path, context)
    {
        _serviceDiscovery = serviceDiscovery;
    }

    protected override IItem? GetItemImpl()
    {
        var ns = _serviceDiscovery.GetNamespace(ItemName);

        return new NamespaceItem(ParentPath, ns);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        if (GetItem() is NamespaceItem @namespace)
        {
            return _serviceDiscovery.ListServices(ItemName).Select(s => new ServiceItem(Path, s, @namespace.ItemName));
        }

        return Enumerable.Empty<IItem>();
    }
}