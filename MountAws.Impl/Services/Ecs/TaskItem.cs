using System.Collections.Immutable;
using MountAnything;
using Task = Amazon.ECS.Model.Task;

namespace MountAws.Services.Ecs;

public class TaskItem : AwsItem<Task>
{

    public TaskItem(ItemPath parentPath, Task task, LinkGenerator linkGenerator) : base(parentPath, task)
    {
        ItemName = task.TaskArn.Split("/").Last();
        LinkPaths = ImmutableDictionary.Create<string,ItemPath>()
            .Add("TaskDefinition", linkGenerator.TaskDefinition(task.TaskDefinitionArn));
    }

    public override string ItemName { get; }
    public override string ItemType => EcsItemTypes.Task;
    public override bool IsContainer => false;
}