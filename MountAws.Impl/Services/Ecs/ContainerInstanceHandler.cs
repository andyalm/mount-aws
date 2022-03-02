using Amazon.EC2;
using Amazon.ECS;
using MountAnything;
using MountAws.Api.AwsSdk.Ecs;
using MountAws.Services.Ec2;

namespace MountAws.Services.Ecs;

public class ContainerInstanceHandler : PathHandler
{
    public static List<string> Include = new() { "TAGS", "CONTAINER_INSTANCE_HEALTH" };

    private readonly IAmazonECS _ecs;
    private readonly IAmazonEC2 _ec2;
    private readonly CurrentCluster _currentCluster;

    public ContainerInstanceHandler(ItemPath path, IPathHandlerContext context, IAmazonECS ecs, IAmazonEC2 ec2, CurrentCluster currentCluster) : base(path, context)
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
            var ec2InstanceId = containerInstance.Ec2InstanceId;
            var ec2Item = ec2InstanceId == null ? null : GetEC2Item(ec2InstanceId);
            return new ContainerInstanceItem(ParentPath, containerInstance, ec2Item);
        }
        catch (ContainerInstanceNotFoundException)
        {
            return null;
        }
    }

    private IItem? GetByEc2InstanceId()
    {
        var containerInstance = _ecs.QueryContainerInstances(_currentCluster.Name, ItemName).FirstOrDefault();
        if (containerInstance == null)
        {
            return null;
        }
        
        var ec2Item = GetEC2Item(containerInstance.Ec2InstanceId);
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
        var ec2Image = _ec2.DescribeImageOrDefault(ec2Instance.ImageId);
        
        return LinkGenerator.Ec2Instance(ec2Instance, ec2Image);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        var item = GetItem() as ContainerInstanceItem;
        if (item == null)
        {
            return Enumerable.Empty<Item>();
        }
        
        var taskArns = _ecs.ListTasksByContainerInstance(_currentCluster.Name, item.ItemName);

        return taskArns.Chunk(100).SelectMany(taskArnChunk =>
        {
            return _ecs.DescribeTasks(_currentCluster.Name, taskArnChunk.ToList(), new[] { "TAGS" });
        }).Select(t => new TaskItem(Path, t, LinkGenerator));
    }
}