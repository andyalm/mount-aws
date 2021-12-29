using System.Collections.Immutable;
using System.Management.Automation;
using MountAnything;

namespace MountAws.Services.Ecs;

public class ServiceItem : AwsItem
{
    public ServiceItem(string parentPath, PSObject service, LinkGenerator linkGenerator) : base(parentPath, service)
    {
        ItemName = Property<string>("ServiceName")!;
        LinkPaths = ImmutableDictionary.Create<string,string>()
            .Add("TaskDefinition", linkGenerator.TaskDefinition(Property<string>("TaskDefinition")!));
    }

    public override string ItemName { get; }
    public override string ItemType => EcsItemTypes.Service;
    public override bool IsContainer => true;
}