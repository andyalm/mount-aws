using Amazon.ECS;
using MountAnything;
using MountAws.Api.AwsSdk.Ecs;
using MountAws.Services.Core;

namespace MountAws.Services.Ecs;

public class TaskDefinitionsHandler : PathHandler
{
    private readonly IAmazonECS _ecs;

    public static IItem CreateItem(string parentPath)
    {
        return new GenericContainerItem(parentPath, "task-definitions",
            "Navigate the task families in the current account and region");
    }
    
    public TaskDefinitionsHandler(string path, IPathHandlerContext context, IAmazonECS ecs) : base(path, context)
    {
        _ecs = ecs;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        int pageSize = 100;
        return _ecs.ListTaskFamilies(null, pageSize)
            .Select(t => new TaskFamilyItem(Path, t))
            .WarnIfMoreItemsThan(1000, Context, "Not all task families were returned because there are too many. Use the -filter argument to scope the results");
    }

    public override IEnumerable<IItem> GetChildItems(string filter)
    {
        return _ecs.ListTaskFamilies(filter.Replace("*", ""))
            .Select(t => new TaskFamilyItem(Path, t));
    }

    // there could be too many of them to make it worth caching
    protected override bool CacheChildren => false;
}