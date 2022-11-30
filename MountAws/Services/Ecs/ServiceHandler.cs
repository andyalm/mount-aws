using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.ECS;
using MountAnything;
using MountAws.Api.AwsSdk.Ecs;
using MountAws.Services.Ec2;

namespace MountAws.Services.Ecs;

public class ServiceHandler : PathHandler, IRemoveItemHandler
{
    private readonly IAmazonECS _ecs;
    private readonly IAmazonEC2 _ec2;
    private readonly CurrentCluster _currentCluster;

    public ServiceHandler(ItemPath path, IPathHandlerContext context, IAmazonECS ecs, IAmazonEC2 ec2, CurrentCluster currentCluster) : base(path, context)
    {
        _ecs = ecs;
        _ec2 = ec2;
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

        var tasks = taskArns.Chunk(10).SelectMany(taskArnChunk =>
        {
            return _ecs.DescribeTasks(_currentCluster.Name,
                taskArnChunk,
                new[] { "TAGS" });
        });
        
        var containerInstanceArns = tasks
            .Where(t => !string.IsNullOrEmpty(t.ContainerInstanceArn))
            .Select(t => t.ContainerInstanceArn)
            .Distinct()
            .ToArray();
        Dictionary<string, Instance> ec2InstancesByContainerInstanceArn = new Dictionary<string, Instance>();
        if (containerInstanceArns.Any())
        {
            var containerInstances = _ecs.DescribeContainerInstances(_currentCluster.Name, containerInstanceArns)
                .Where(c => !string.IsNullOrEmpty(c.Ec2InstanceId))
                .ToDictionary(i => i.Ec2InstanceId);

            var ec2InstanceIds = containerInstances.Values.Select(c => c.Ec2InstanceId);
            ec2InstancesByContainerInstanceArn = _ec2.GetInstancesByIds(ec2InstanceIds)
                .Where(instance => containerInstances.ContainsKey(instance.InstanceId))
                .ToDictionary(i => containerInstances[i.InstanceId].ContainerInstanceArn);
        }

        return tasks.Select(t => new TaskItem(Path, t,
            t.ContainerInstanceArn != null
                ? ec2InstancesByContainerInstanceArn.GetValueOrDefault(t.ContainerInstanceArn)
                : null, LinkGenerator, useServiceView:true));
    }

    public void RemoveItem()
    {
        _ecs.DeleteService(_currentCluster.Name, ItemName, Context.Force);
    }
}