using Amazon.ServiceDiscovery;
using MountAnything;

namespace MountAws.Services.ServiceDiscovery;

public class ServiceHandler : PathHandler
{
    private readonly IAmazonServiceDiscovery _serviceDiscovery;

    public ServiceHandler(ItemPath path, IPathHandlerContext context, IAmazonServiceDiscovery serviceDiscovery) : base(path, context)
    {
        _serviceDiscovery = serviceDiscovery;
    }

    protected override IItem? GetItemImpl()
    {
        var service = _serviceDiscovery.GetService(ItemName);

        return new ServiceItem(ParentPath, service);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        if (GetItem() is ServiceItem serviceItem)
        {
            return _serviceDiscovery.ListInstances(ItemName).Select(i => new InstanceItem(Path, i, LinkGenerator, serviceItem.NamespaceId, serviceItem.ItemName));
        }
        
        return Enumerable.Empty<IItem>();
    }
}