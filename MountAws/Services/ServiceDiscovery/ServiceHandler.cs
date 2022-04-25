using Amazon.ServiceDiscovery;
using Amazon.ServiceDiscovery.Model;
using MountAnything;

namespace MountAws.Services.ServiceDiscovery;

public class ServiceHandler : PathHandler
{
    private readonly IAmazonServiceDiscovery _serviceDiscovery;
    private readonly IItemAncestor<NamespaceItem> _namespaceItem;

    public ServiceHandler(ItemPath path, IPathHandlerContext context, IAmazonServiceDiscovery serviceDiscovery, IItemAncestor<NamespaceItem> namespaceItem) : base(path, context)
    {
        _serviceDiscovery = serviceDiscovery;
        _namespaceItem = namespaceItem;
    }

    private string ServiceId => Cache.ResolveAlias<ServiceItem>(ItemName, ServiceIdFromName);

    protected override IItem? GetItemImpl()
    {
        try
        {
            var service = _serviceDiscovery.GetService(ServiceId);

            return new ServiceItem(ParentPath, service);
        }
        catch (ServiceNotFoundException)
        {
            return null;
        }
        
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        if (GetItem() is ServiceItem serviceItem)
        {
            return _serviceDiscovery.ListInstances(ServiceId).Select(i => new InstanceItem(Path, i, LinkGenerator, serviceItem.NamespaceId, serviceItem.ItemName));
        }
        
        return Enumerable.Empty<IItem>();
    }
    
    private string ServiceIdFromName(string serviceName)
    {
        WriteDebug($"ServiceIdFromName({serviceName})");
        var ns = _serviceDiscovery.ListServices(_namespaceItem.Item.ItemName)
            .SingleOrDefault(n => n.Name.Equals(serviceName, StringComparison.OrdinalIgnoreCase));

        if (ns == null)
        {
            throw new ServiceNotFoundException($"Service with name '{serviceName}' could not be found");
        }

        return ns.Id;
    }
}