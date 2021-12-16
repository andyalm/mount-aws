using Amazon.ECS;
using Amazon.ECS.Model;
using MountAnything;

using static MountAws.PagingHelper;

namespace MountAws.Services.ECS;

public class ServiceHandler : PathHandler
{
    private readonly IAmazonECS _ecs;
    private readonly CurrentCluster _currentCluster;

    public ServiceHandler(string path, IPathHandlerContext context, IAmazonECS ecs, CurrentCluster currentCluster) : base(path, context)
    {
        _ecs = ecs;
        _currentCluster = currentCluster;
    }

    protected override Item? GetItemImpl()
    {
        var service = _ecs.DescribeServicesAsync(new DescribeServicesRequest
        {
            Cluster = _currentCluster.Name,
            Services = new List<string> { ItemName }
        }).GetAwaiter().GetResult().Services.FirstOrDefault();

        if (service != null)
        {
            return new ServiceItem(ParentPath, service);
        }

        return null;

    }

    protected override IEnumerable<Item> GetChildItemsImpl()
    {
        var taskArns = GetWithPaging(nextToken =>
        {
            var response = _ecs.ListTasksAsync(new ListTasksRequest
            {
                Cluster = _currentCluster.Name,
                ServiceName = ItemName,
                NextToken = nextToken
            }).GetAwaiter().GetResult();

            return new PaginatedResponse<string>
            {
                PageOfResults = response.TaskArns.ToArray(),
                NextToken = nextToken
            };
        });

        return taskArns.Chunk(10).SelectMany(taskArnChunk =>
        {
            return _ecs.DescribeTasksAsync(new DescribeTasksRequest
            {
                Cluster = _currentCluster.Name,
                Include = new List<string> { "TAGS" },
                Tasks = taskArnChunk.ToList()
            }).GetAwaiter().GetResult().Tasks;
        }).Select(t => new TaskItem(Path, t));
    }
}