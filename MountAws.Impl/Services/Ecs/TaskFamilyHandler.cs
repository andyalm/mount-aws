using Amazon.ECS;
using MountAnything;
using MountAws.Api.AwsSdk.Ecs;

namespace MountAws.Services.Ecs;

public class TaskFamilyHandler : PathHandler
{
    private readonly IAmazonECS _ecs;

    public TaskFamilyHandler(ItemPath path, IPathHandlerContext context, IAmazonECS ecs) : base(path, context)
    {
        _ecs = ecs;
    }

    protected override IItem? GetItemImpl()
    {
        //TODO: verify existence
        return new TaskFamilyItem(ParentPath, ItemName);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        var inactiveTaskDefinitions = Enumerable.Empty<TaskDefinitionItem>();
        if (Context.Force)
        {
            inactiveTaskDefinitions = GetTaskDefinitions(false);
        }

        var activeTaskDefinitions = GetTaskDefinitions(true);

        return inactiveTaskDefinitions.Concat(activeTaskDefinitions);
    }

    private IEnumerable<TaskDefinitionItem> GetTaskDefinitions(bool isActive)
    {
        return _ecs.ListTaskDefinitionsByFamily(ItemName, isActive)
            .Select(arn => new TaskDefinitionItem(Path, arn));
    }
}