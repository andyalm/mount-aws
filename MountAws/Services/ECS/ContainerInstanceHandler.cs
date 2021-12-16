using Amazon.ECS;
using Amazon.ECS.Model;
using MountAnything;

using static MountAws.PagingHelper;

namespace MountAws.Services.ECS;

public class ContainerInstanceHandler : PathHandler
{
    public static List<string> Include = new() { "TAGS", "CONTAINER_INSTANCE_HEALTH" };

    private readonly IAmazonECS _ecs;
    private readonly CurrentCluster _currentCluster;

    public ContainerInstanceHandler(string path, IPathHandlerContext context, IAmazonECS ecs, CurrentCluster currentCluster) : base(path, context)
    {
        _ecs = ecs;
        _currentCluster = currentCluster;
    }

    protected override Item? GetItemImpl()
    {
        if (ItemName.StartsWith("i-"))
        {
            return GetByEc2InstanceId();
        }
        
        var containerInstance = _ecs.DescribeContainerInstancesAsync(new DescribeContainerInstancesRequest
        {
            Cluster = _currentCluster.Name,
            ContainerInstances = new List<string> { ItemName },
            Include = Include
        }).GetAwaiter().GetResult().ContainerInstances.FirstOrDefault();
        if (containerInstance != null)
        {
            return new ContainerInstanceItem(ParentPath, containerInstance);
        }

        return null;
    }

    private Item? GetByEc2InstanceId()
    {
        var containerInstance = _ecs.QueryContainerInstances(_currentCluster.Name, ItemName).FirstOrDefault();

        return containerInstance != null ? new ContainerInstanceItem(ParentPath, containerInstance) : null;
    }

    protected override IEnumerable<Item> GetChildItemsImpl()
    {
        var item = GetItem() as ContainerInstanceItem;
        if (item == null)
        {
            return Enumerable.Empty<Item>();
        }
        
        var taskArns = GetWithPaging(nextToken =>
        {
            var response = _ecs.ListTasksAsync(new ListTasksRequest
            {
                Cluster = _currentCluster.Name,
                ContainerInstance = item.ItemName
            }).GetAwaiter().GetResult();

            return new PaginatedResponse<string>
            {
                PageOfResults = response.TaskArns.ToArray(),
                NextToken = nextToken
            };
        });

        return taskArns.Chunk(100).SelectMany(taskArnChunk =>
        {
            return _ecs.DescribeTasksAsync(new DescribeTasksRequest
            {
                Cluster = _currentCluster.Name,
                Tasks = taskArnChunk.ToList(),
                Include = new List<string> { "TAGS" }
            }).GetAwaiter().GetResult().Tasks;
        }).Select(t => new TaskItem(Path, t));
    }
}