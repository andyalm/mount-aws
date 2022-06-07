using Amazon.ECS;
using Amazon.ECS.Model;
using MountAnything;
using MountAws.Api.AwsSdk.Ecs;

namespace MountAws.Services.Ecs;

public class TaskDefinitionHandler : PathHandler
{
    private readonly IAmazonECS _ecs;

    public TaskDefinitionHandler(ItemPath path, IPathHandlerContext context, IAmazonECS ecs) : base(path, context)
    {
        _ecs = ecs;
    }

    protected override IItem? GetItemImpl()
    {
        var (taskDefinition, tags) = GetTaskDefinition();

        return new TaskDefinitionItem(ParentPath, taskDefinition, tags);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield break;
    }

    private (TaskDefinition TaskDefinition, Tag[] Tags) GetTaskDefinition()
    {
        var family = ParentPath.Name;
        var taskDefinitionName = $"{family}:{ItemName}";
        return _ecs.DescribeTaskDefinition(taskDefinitionName);
    }
}