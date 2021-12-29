using System.Collections.Immutable;
using System.Management.Automation;
using MountAnything;

namespace MountAws.Services.Ecs;

public class TaskItem : AwsItem
{

    public TaskItem(string parentPath, PSObject task, LinkGenerator linkGenerator) : base(parentPath, task)
    {
        ItemName = Property<string>("TaskArn")!.Split("/").Last();
        LinkPaths = ImmutableDictionary.Create<string,string>()
            .Add("TaskDefinition", linkGenerator.TaskDefinition(Property<string>("TaskDefinitionArn")!));
    }

    public override string ItemName { get; }
    public override string ItemType => EcsItemTypes.Task;
    public override bool IsContainer => false;
}