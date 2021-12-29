using MountAnything;
using MountAws.Api.Ecs;
using static MountAws.PagingHelper;

namespace MountAws.Services.Ecs;

public class ServiceHandler : PathHandler, IRemoveItemHandler
{
    private readonly IEcsApi _ecs;
    private readonly CurrentCluster _currentCluster;

    public ServiceHandler(string path, IPathHandlerContext context, IEcsApi ecs, CurrentCluster currentCluster) : base(path, context)
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
        var taskArns = GetWithPaging(nextToken =>
        {
            var response = _ecs.ListTasksByService(_currentCluster.Name,
                ItemName,
                nextToken
            );

            return new PaginatedResponse<string>
            {
                PageOfResults = response.TaskArns,
                NextToken = nextToken
            };
        });

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