using System.Management.Automation;
using System.Text.RegularExpressions;
using MountAnything;
using MountAws.Api.Ec2;
using MountAws.Api.Ecs;
using MountAws.Services.Core;
using MountAws.Services.Ec2;

namespace MountAws.Services.Ecs;

public class ContainerInstancesHandler : PathHandler
{
    private readonly IEcsApi _ecs;
    private readonly IEc2Api _ec2;
    private readonly CurrentCluster _currentCluster;

    public static Item CreateItem(string parentPath)
    {
        return new GenericContainerItem(parentPath, "container-instances",
            "Navigate the container instances within the ecs cluster");
    }
    
    public ContainerInstancesHandler(string path, IPathHandlerContext context, IEcsApi ecs, IEc2Api ec2, CurrentCluster currentCluster) : base(path, context)
    {
        _ecs = ecs;
        _ec2 = ec2;
        _currentCluster = currentCluster;
    }

    protected override Item? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<Item> GetChildItemsImpl()
    {
        var containerInstances = _ecs.QueryContainerInstances(_currentCluster.Name).ToArray();

        return ToItems(containerInstances);
    }

    private static Regex _containerInstanceIdRegex = new(@"^[a-z0-9\*]+$");
    public override IEnumerable<Item> GetChildItems(string filter)
    {
        // ECS api can't find container instance by partial id. So defer to base implementation that relies on scanning all children
        if (_containerInstanceIdRegex.IsMatch(filter))
        {
            return base.GetChildItems(filter);
        }
        
        var containerInstances = _ecs.QueryContainerInstances(_currentCluster.Name, filter).ToArray();

        return ToItems(containerInstances);
    }

    private IEnumerable<Item> ToItems(PSObject[] containerInstances)
    {
        var ec2InstanceIds = containerInstances
            .Select(i => i.Property<string>("Ec2InstanceId"))
            .Where(i => !string.IsNullOrEmpty(i)).Cast<string>().Distinct();

        var ec2InstancesById = _ec2.GetInstancesByIds(ec2InstanceIds).ToDictionary(i => i.Property<string>("InstanceId")!);

        return containerInstances.Select(containerInstance =>
        {
            var ec2InstanceId = containerInstance.Property<string>("Ec2InstanceId");
            var ec2Item = string.IsNullOrEmpty(ec2InstanceId) && ec2InstancesById.TryGetValue(ec2InstanceId!, out var ec2Instance)
                ? LinkGenerator.EC2Instance(ec2Instance)
                : null;

            return new ContainerInstanceItem(Path, containerInstance, ec2Item);
        });
    }
}