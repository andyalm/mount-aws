using System.Collections.Immutable;
using Amazon.ECS.Model;
using MountAnything;
using MountAws.Services.Ec2;

namespace MountAws.Services.Ecs;

public class ContainerInstanceItem : AwsItem<ContainerInstance>
{
    public InstanceItem? Ec2Instance { get; }
    
    public ContainerInstanceItem(ItemPath parentPath, ContainerInstance containerInstance, InstanceItem? ec2Instance) : base(parentPath, containerInstance)
    {
        ItemName = containerInstance.Id();
        if (ec2Instance != null)
        {
            Links = ImmutableDictionary.Create<string,IItem>().Add("Ec2Instance", ec2Instance);
        }

        Ec2Instance = ec2Instance;
    }

    public override string ItemName { get; }
    public override string ItemType => EcsItemTypes.ContainerInstance;

    public override string? WebUrl =>
        UrlBuilder.CombineWith($"ecs/home#/clusters/{UnderlyingObject.ClusterName()}/containerInstances/{ItemName}");
    public override bool IsContainer => true;

    public override IEnumerable<string> Aliases
    {
        get
        {
            var ec2InstanceId = UnderlyingObject.Ec2InstanceId;
            if (ec2InstanceId != null)
            {
                yield return ec2InstanceId;
            }

            var privateIpAddress = Ec2Instance?.UnderlyingObject.PrivateIpAddress;
            if (privateIpAddress != null)
            {
                yield return privateIpAddress;
            }
        }
    }
    
    [ItemProperty]
    public string? PrivateIpAddress => Ec2Instance?.UnderlyingObject.PrivateIpAddress;

    [ItemProperty] 
    public string? InstanceType => Ec2Instance?.UnderlyingObject.InstanceType.Value;
}