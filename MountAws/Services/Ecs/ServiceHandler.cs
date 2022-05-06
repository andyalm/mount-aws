using Amazon.ECS;
using MountAnything;
using MountAws.Api.AwsSdk.Ecs;
using static MountAws.PagingHelper;

namespace MountAws.Services.Ecs;

public class ServiceHandler : PathHandler, IRemoveItemHandler
{
    private readonly IAmazonECS _ecs;
    private readonly CurrentCluster _currentCluster;

    public ServiceHandler(ItemPath path, IPathHandlerContext context, IAmazonECS ecs, CurrentCluster currentCluster) : base(path, context)
    {
        _ecs = ecs;
        _currentCluster = currentCluster;
    }

    protected override IItem? GetItemImpl()
    {
        var service = _ecs.DescribeServices(_currentCluster.Name,
            new[] { ItemName },
            new[] { "TAGS" }).FirstOrDefault();
        
        if (service != null)
        {
            return new ServiceItem(ParentPath, service, LinkGenerator);
        }

        return null;
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        var taskArns = _ecs.ListTasksByService(_currentCluster.Name, ItemName);

        return taskArns.Chunk(10).SelectMany(taskArnChunk =>
        {
            return _ecs.DescribeTasks(_currentCluster.Name,
                taskArnChunk,
                new[] { "TAGS" });
        }).Select(t => new TaskItem(Path, t, LinkGenerator));
    }

    public void RemoveItem()
    {
        _ecs.DeleteService(_currentCluster.Name, ItemName, Context.Force);
    }
}