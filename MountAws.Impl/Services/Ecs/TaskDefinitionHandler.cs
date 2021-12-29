using Amazon.ECS;
using MountAnything;
using MountAws.Api.AwsSdk.Ecs;

namespace MountAws.Services.Ecs;

public class TaskDefinitionHandler : PathHandler
{
    private readonly IAmazonECS _ecs;

    public TaskDefinitionHandler(string path, IPathHandlerContext context, IAmazonECS ecs) : base(path, context)
    {
        _ecs = ecs;
    }

    protected override IItem? GetItemImpl()
    {
        var family = ItemPath.GetLeaf(ParentPath);
        var taskDefinitionName = $"{family}:{ItemName}";
        var taskDefinition = _ecs.DescribeTaskDefinition(taskDefinitionName);

        return new TaskDefinitionItem(ParentPath, taskDefinition.TaskDefinition, taskDefinition.Tags);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield break;
    }
}