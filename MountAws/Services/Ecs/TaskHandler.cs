using System.Management.Automation;
using MountAnything;
using MountAws.Api.Ecs;

namespace MountAws.Services.Ecs;

public class TaskHandler : PathHandler, IRemoveItemHandler, IRemoveItemParameters<RemoveTaskParameters>
{
    private readonly IEcsApi _ecs;
    private readonly CurrentCluster _currentCluster;

    public TaskHandler(string path, IPathHandlerContext context, IEcsApi ecs, CurrentCluster currentCluster) : base(path, context)
    {
        _ecs = ecs;
        _currentCluster = currentCluster;
    }

    protected override IItem? GetItemImpl()
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

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield break;
    }

    public void RemoveItem()
    {
        _ecs.StopTask(_currentCluster.Name, ItemName, RemoveItemParameters?.Reason);
    }

    public RemoveTaskParameters? RemoveItemParameters { get; set; }
}

public class RemoveTaskParameters
{
    [Parameter(Mandatory = false)]
    public string? Reason { get; set; }
}