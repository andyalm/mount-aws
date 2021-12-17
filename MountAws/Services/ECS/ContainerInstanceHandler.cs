using Amazon.EC2;
using Amazon.ECS;
using Amazon.ECS.Model;
using MountAnything;
using MountAws.Services.EC2;
using static MountAws.PagingHelper;

namespace MountAws.Services.ECS;

public class ContainerInstanceHandler : PathHandler
{
    public static List<string> Include = new() { "TAGS", "CONTAINER_INSTANCE_HEALTH" };

    private readonly IAmazonECS _ecs;
    private readonly IAmazonEC2 _ec2;
    private readonly CurrentCluster _currentCluster;

    public ContainerInstanceHandler(string path, IPathHandlerContext context, IAmazonECS ecs, IAmazonEC2 ec2, CurrentCluster currentCluster) : base(path, context)
    {
        _ecs = ecs;
        _ec2 = ec2;
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
            var ec2Item = GetEC2Item(containerInstance.Ec2InstanceId);
            
            return new ContainerInstanceItem(ParentPath, containerInstance, ec2Item);
        }

        return null;
    }

    private Item? GetByEc2InstanceId()
    {
        var containerInstance = _ecs.QueryContainerInstances(_currentCluster.Name, ItemName).FirstOrDefault();
        if (containerInstance == null)
        {
            return null;
        }

        var ec2Item = GetEC2Item(containerInstance.Ec2InstanceId);
        return new ContainerInstanceItem(ParentPath, containerInstance, ec2Item);
    }

    private EC2InstanceItem? GetEC2Item(string ec2InstanceId)
    {
        if (string.IsNullOrEmpty(ec2InstanceId))
        {
            return null;
        }
        
        var ec2Instance = _ec2.QueryInstances(ec2InstanceId).SingleOrDefault();
        if (ec2Instance == null)
        {
            return null;
        }
        
        return LinkGenerator.EC2Instance(ec2Instance);
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