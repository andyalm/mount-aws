using System.Management.Automation;
using Amazon.ServiceDiscovery.Model;
using MountAnything;

namespace MountAws.Services.ServiceDiscovery;

public class InstanceItem : AwsItem
{
    private readonly LinkGenerator _linkGenerator;

    public InstanceItem(ItemPath parentPath, InstanceSummary instanceSummary, LinkGenerator linkGenerator, string namespaceId, string serviceId) : base(parentPath, new PSObject(instanceSummary))
    {
        _linkGenerator = linkGenerator;
        ItemName = instanceSummary.Id;
        Attributes = instanceSummary.Attributes;
        NamespaceId = namespaceId;
        ServiceId = serviceId;
        LinkPaths = CreateLinks();
    }
    
    public InstanceItem(ItemPath parentPath, Instance instance, LinkGenerator linkGenerator, string namespaceId, string serviceId) : base(parentPath, new PSObject(instance))
    {
        _linkGenerator = linkGenerator;
        ItemName = instance.Id;
        Attributes = instance.Attributes;
        NamespaceId = namespaceId;
        ServiceId = serviceId;
        LinkPaths = CreateLinks();
    }

    public override string ItemName { get; }
    public override bool IsContainer => false;
    public string NamespaceId { get; set; }
    public string ServiceId { get; set; }
    public Dictionary<string,string> Attributes { get; }

    public override string? WebUrl =>
        UrlBuilder.CombineWith($"cloudmap/home/namespaces/{NamespaceId}/services/{ServiceId}/instances/{ItemName}");

    protected override void CustomizePSObject(PSObject psObject)
    {
        foreach (var attribute in Attributes)
        {
            psObject.Properties.Add(new PSNoteProperty(attribute.Key.SnakeToPascalCase(), attribute.Value));
        }
        base.CustomizePSObject(psObject);
    }

    private Dictionary<string, ItemPath> CreateLinks()
    {
        var links = new Dictionary<string, ItemPath>();
        if (Attributes.TryGetValue("ECS_CLUSTER_NAME", out var clusterName))
        {
            links["EcsCluster"] = _linkGenerator.EcsCluster(clusterName);
            
            if (Attributes.TryGetValue("ECS_SERVICE_NAME", out var ecsService))
            {
                links["EcsService"] = _linkGenerator.EcsService(clusterName, ecsService);
            }
        }

        return links;
    }
}