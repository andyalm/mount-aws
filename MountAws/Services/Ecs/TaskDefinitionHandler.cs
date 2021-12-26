using MountAnything;
using MountAws.Api.Ecs;

namespace MountAws.Services.Ecs;

public class TaskDefinitionHandler : PathHandler
{
    private readonly IEcsApi _ecs;

    public TaskDefinitionHandler(string path, IPathHandlerContext context, IEcsApi ecs) : base(path, context)
    {
        _ecs = ecs;
    }

    protected override IItem? GetItemImpl()
    {
        var family = ItemPath.GetLeaf(ParentPath);
        var taskDefinitionName = $"{family}:{ItemName}";
        var taskDefinition = _ecs.DescribeTaskDefinition(taskDefinitionName);

        return new TaskDefinitionItem(ParentPath, taskDefinition);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield break;
    }
}