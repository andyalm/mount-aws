using Amazon.ServiceDiscovery;
using MountAnything;

namespace MountAws.Services.ServiceDiscovery;

public class InstanceHandler : PathHandler
{
    private readonly IAmazonServiceDiscovery _serviceDiscovery;
    private readonly ServiceItem _serviceItem;

    public InstanceHandler(ItemPath path, IPathHandlerContext context, IAmazonServiceDiscovery serviceDiscovery, ServiceItem serviceItem) : base(path, context)
    {
        _serviceDiscovery = serviceDiscovery;
        _serviceItem = serviceItem;
    }

    protected override IItem? GetItemImpl()
    {
        var instance = _serviceDiscovery.GetInstance(ParentPath.Name, ItemName);

        return new InstanceItem(ParentPath, instance, LinkGenerator, _serviceItem.NamespaceId, _serviceItem.ItemName);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield break;
    }
}