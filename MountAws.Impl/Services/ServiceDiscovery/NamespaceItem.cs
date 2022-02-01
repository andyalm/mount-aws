using System.Management.Automation;
using Amazon.ServiceDiscovery.Model;
using MountAnything;

namespace MountAws.Services.ServiceDiscovery;

public class NamespaceItem : AwsItem
{
    public NamespaceItem(ItemPath parentPath, NamespaceSummary underlyingObject) : base(parentPath, new PSObject(underlyingObject))
    {
        ItemName = underlyingObject.Id;
    }

    public NamespaceItem(ItemPath parentPath, Namespace underlyingObject) : base(parentPath,
        new PSObject(underlyingObject))
    {
        ItemName = underlyingObject.Id;
    }

    public override string ItemName { get; }
    public override bool IsContainer => true;
    public override string? WebUrl => UrlBuilder.CombineWith($"cloudmap/home/namespaces/{ItemName}");
}