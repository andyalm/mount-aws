using System.Collections.Immutable;
using System.Management.Automation;
using Amazon.ECS.Model;
using MountAnything;

namespace MountAws.Services.Ecs;

public class ServiceItem : AwsItem<Service>
{
    public ServiceItem(string parentPath, Service service, LinkGenerator linkGenerator) : base(parentPath, service)
    {
        ItemName = service.ServiceName;
        LinkPaths = ImmutableDictionary.Create<string,string>()
            .Add("TaskDefinition", linkGenerator.TaskDefinition(service.TaskDefinition));
    }

    public override string ItemName { get; }
    public override string ItemType => EcsItemTypes.Service;
    public override bool IsContainer => true;
}