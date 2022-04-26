using System.Management.Automation;
using Amazon.ECS;
using MountAnything;
using MountAws.Api.AwsSdk.Ecs;

namespace MountAws.Services.Ecs;

public class TaskHandler : PathHandler, IRemoveItemHandler, IRemoveItemParameters<RemoveTaskParameters>
{
    private readonly IAmazonECS _ecs;
    private readonly CurrentCluster _currentCluster;

    public TaskHandler(ItemPath path, IPathHandlerContext context, IAmazonECS ecs, CurrentCluster currentCluster) : base(path, context)
    {
        _ecs = ecs;
        _currentCluster = currentCluster;
    }

    protected override IItem? GetItemImpl()
    {
        var task = _ecs.DescribeTasks(_currentCluster.Name,
            new[] { ItemName },
            new[] { "TAGS" }).SingleOrDefault();

        if (task != null)
        {
            return new TaskItem(ParentPath, task, LinkGenerator);
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