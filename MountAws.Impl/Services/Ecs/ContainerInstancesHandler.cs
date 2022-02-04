using System.Text.RegularExpressions;
using Amazon.EC2;
using Amazon.ECS;
using Amazon.ECS.Model;
using MountAnything;
using MountAws.Api.AwsSdk.Ecs;
using MountAws.Services.Core;
using MountAws.Services.Ec2;

namespace MountAws.Services.Ecs;

public class ContainerInstancesHandler : PathHandler
{
    private readonly IAmazonECS _ecs;
    private readonly IAmazonEC2 _ec2;
    private readonly CurrentCluster _currentCluster;

    public static Item CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "container-instances",
            "Navigate the container instances within the ecs cluster");
    }
    
    public ContainerInstancesHandler(ItemPath path, IPathHandlerContext context, IAmazonECS ecs, IAmazonEC2 ec2, CurrentCluster currentCluster) : base(path, context)
    {
        _ecs = ecs;
        _ec2 = ec2;
        _currentCluster = currentCluster;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        var containerInstances = _ecs.QueryContainerInstances(_currentCluster.Name).ToArray();

        return ToItems(containerInstances);
    }

    private static Regex _containerInstanceIdRegex = new(@"^[a-z0-9\*]+$");
    public override IEnumerable<IItem> GetChildItems(string filter)
    {
        // ECS api can't find container instance by partial id. So defer to base implementation that relies on scanning all children
        if (_containerInstanceIdRegex.IsMatch(filter))
        {
            return base.GetChildItems(filter);
        }
        
        var containerInstances = _ecs.QueryContainerInstances(_currentCluster.Name, filter).ToArray();

        return ToItems(containerInstances);
    }

    private IEnumerable<IItem> ToItems(ContainerInstance[] containerInstances)
    {
        var ec2InstanceIds = containerInstances
            .Select(i => i.Ec2InstanceId)
            .Where(i => !string.IsNullOrEmpty(i)).Distinct();

        var ec2InstancesById = _ec2.GetInstancesByIds(ec2InstanceIds).ToDictionary(i => i.InstanceId);

        return containerInstances.Select(containerInstance =>
        {
            var ec2InstanceId = containerInstance.Ec2InstanceId;
            var ec2Item = !string.IsNullOrEmpty(ec2InstanceId) && ec2InstancesById.TryGetValue(ec2InstanceId!, out var ec2Instance)
                ? LinkGenerator.Ec2Instance(ec2Instance)
                : null;

            return new ContainerInstanceItem(Path, containerInstance, ec2Item);
        });
    }
}