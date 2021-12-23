using MountAnything;
using MountAws.Api.Ecs;

namespace MountAws.Services.Ecs;

public class TaskHandler : PathHandler
{
    private readonly IEcsApi _ecs;
    private readonly CurrentCluster _currentCluster;

    public TaskHandler(string path, IPathHandlerContext context, IEcsApi ecs, CurrentCluster currentCluster) : base(path, context)
    {
        _ecs = ecs;
        _currentCluster = currentCluster;
    }

    protected override Item? GetItemImpl()
    {
        var task = _ecs.DescribeTasks(_currentCluster.Name,
            new[] { "TAGS" },
            new[] { ItemName }).SingleOrDefault();

        if (task != null)
        {
            return new TaskItem(ParentPath, task);
        }

        return null;
    }

    protected override IEnumerable<Item> GetChildItemsImpl()
    {
        yield break;
    }
}