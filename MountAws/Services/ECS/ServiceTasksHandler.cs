using Amazon.ECS;
using Amazon.ECS.Model;
using MountAnything;
using MountAws.Services.Core;

using static MountAws.PagingHelper;

namespace MountAws.Services.ECS;

public class ServiceTasksHandler : PathHandler
{
    private readonly IAmazonECS _ecs;
    private readonly CurrentCluster _currentCluster;
    private readonly ServiceHandler _serviceHandler;
    public string ServiceName { get; }

    public static Item CreateItem(string parentPath)
    {
        return new GenericContainerItem(parentPath, "tasks",
            "Navigate the ECS tasks of the service");
    }
    
    public ServiceTasksHandler(string path, IPathHandlerContext context, IAmazonECS ecs, CurrentCluster currentCluster) : base(path, context)
    {
        _ecs = ecs;
        _currentCluster = currentCluster;
        _serviceHandler = new ServiceHandler(ParentPath, Context, _ecs, _currentCluster);
        ServiceName = ItemPath.GetLeaf(_serviceHandler.Path);
    }

    protected override bool ExistsImpl()
    {
        return _serviceHandler.Exists();
    }

    protected override Item? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<Item> GetChildItemsImpl()
    {
        var taskArns = GetWithPaging(nextToken =>
        {
            var response = _ecs.ListTasksAsync(new ListTasksRequest
            {
                Cluster = _currentCluster.Name,
                ServiceName = ServiceName,
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