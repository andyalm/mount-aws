using System.Management.Automation;
using Amazon.ServiceDiscovery.Model;
using MountAnything;

namespace MountAws.Services.ServiceDiscovery;

public class ServiceItem : AwsItem
{
    public ServiceItem(ItemPath parentPath, ServiceSummary underlyingObject, string namespaceId) : base(parentPath, new PSObject(underlyingObject))
    {
        ItemName = underlyingObject.Id;
        NamespaceId = namespaceId;
    }
    
    public ServiceItem(ItemPath parentPath, Service underlyingObject) : base(parentPath, new PSObject(underlyingObject))
    {
        ItemName = underlyingObject.Id;
        NamespaceId = underlyingObject.NamespaceId;
    }

    public override string ItemName { get; }
    public override bool IsContainer => true;
    public string NamespaceId { get; set; }

    public override string? WebUrl =>
        UrlBuilder.CombineWith($"cloudmap/home/namespaces/{NamespaceId}/services/{ItemName}");
}