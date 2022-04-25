using Amazon.ServiceDiscovery;
using Amazon.ServiceDiscovery.Model;
using MountAnything;

namespace MountAws.Services.ServiceDiscovery;

public class NamespaceHandler : PathHandler
{
    private readonly IAmazonServiceDiscovery _serviceDiscovery;

    public NamespaceHandler(ItemPath path, IPathHandlerContext context, IAmazonServiceDiscovery serviceDiscovery) : base(path, context)
    {
        _serviceDiscovery = serviceDiscovery;
    }
    
    private string NamespaceId => Cache.ResolveAlias<NamespaceItem>(ItemName, NamespaceIdFromName);

    protected override IItem? GetItemImpl()
    {
        try
        {
            var ns = _serviceDiscovery.GetNamespace(NamespaceId);

            return new NamespaceItem(ParentPath, ns);
        }
        catch (NamespaceNotFoundException)
        {
            return null;
        }
    }
    
    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        if (GetItem() is NamespaceItem @namespace)
        {
            return _serviceDiscovery.ListServices(NamespaceId).Select(s => new ServiceItem(Path, s, @namespace.ItemName));
        }

        return Enumerable.Empty<IItem>();
    }
    
    private string NamespaceIdFromName(string namespaceName)
    {
        WriteDebug($"NamespaceIdFromName({namespaceName})");
        var ns = _serviceDiscovery.ListNamespaces()
            .SingleOrDefault(n => n.Name.Equals(namespaceName, StringComparison.OrdinalIgnoreCase));

        if (ns == null)
        {
            throw new NamespaceNotFoundException($"Namespace with name '{namespaceName}' could not be found");
        }

        return ns.Id;
    }
}