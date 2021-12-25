using MountAnything;
using MountAws.Api.Ec2;
using MountAws.Api.Ecs;
using MountAws.Services.Ec2;
using static MountAws.PagingHelper;

namespace MountAws.Services.Ecs;

public class ContainerInstanceHandler : PathHandler
{
    public static List<string> Include = new() { "TAGS", "CONTAINER_INSTANCE_HEALTH" };

    private readonly IEcsApi _ecs;
    private readonly IEc2Api _ec2;
    private readonly CurrentCluster _currentCluster;

    public ContainerInstanceHandler(string path, IPathHandlerContext context, IEcsApi ecs, IEc2Api ec2, CurrentCluster currentCluster) : base(path, context)
    {
        _ecs = ecs;
        _ec2 = ec2;
        _currentCluster = currentCluster;
    }

    protected override IItem? GetItemImpl()
    {
        if (ItemName.StartsWith("i-"))
        {
            return GetByEc2InstanceId();
        }

        try
        {
            var containerInstance = _ecs.DescribeContainerInstance(_currentCluster.Name, ItemName, Include);
            var ec2InstanceId = containerInstance.Property<string>("Ec2InstanceId");
            var ec2Item = ec2InstanceId == null ? null : GetEC2Item(ec2InstanceId);
            return new ContainerInstanceItem(ParentPath, containerInstance, ec2Item);
        }
        catch (ContainerInstanceNotFoundException)
        {
            return null;
        }
    }

    private Item? GetByEc2InstanceId()
    {
        var containerInstance = _ecs.QueryContainerInstances(_currentCluster.Name, ItemName).FirstOrDefault();
        if (containerInstance == null)
        {
            return null;
        }
        
        var ec2Item = GetEC2Item(containerInstance.Property<string>("Ec2InstanceId")!);
        return new ContainerInstanceItem(ParentPath, containerInstance, ec2Item);
    }

    private InstanceItem? GetEC2Item(string ec2InstanceId)
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

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        var item = GetItem() as ContainerInstanceItem;
        if (item == null)
        {
            return Enumerable.Empty<Item>();
        }
        
        var taskArns = GetWithPaging(nextToken =>
        {
            var response = _ecs.ListTasksByContainerInstance(_currentCluster.Name, item.ItemName, nextToken);

            return new PaginatedResponse<string>
            {
                PageOfResults = response.TaskArns.ToArray(),
                NextToken = response.NextToken
            };
        });

        return taskArns.Chunk(100).SelectMany(taskArnChunk =>
        {
            return _ecs.DescribeTasks(_currentCluster.Name, taskArnChunk.ToList(), new[] { "TAGS" });
        }).Select(t => new TaskItem(Path, t));
    }
}