using System.Collections.Immutable;
using System.Management.Automation;
using MountAnything;
using MountAws.Services.Ec2;

namespace MountAws.Services.Ecs;

public class ContainerInstanceItem : Item
{
    public ContainerInstanceItem(string parentPath, PSObject containerInstance, InstanceItem? ec2Instance) : base(parentPath, containerInstance)
    {
        ItemName = containerInstance.Property<string>("ContainerInstanceArn")!.Split("/").Last();
        if (ec2Instance != null)
        {
            Links = ImmutableDictionary.Create<string,Item>().Add("Ec2Instance", ec2Instance);
        }
    }

    public override string ItemName { get; }
    public override string ItemType => EcsItemTypes.ContainerInstance;
    public override bool IsContainer => true;

    public override IEnumerable<string> Aliases
    {
        get
        {
            var ec2InstanceId = Property<string>("Ec2InstanceId");
            if (ec2InstanceId != null)
            {
                yield return ec2InstanceId;
            }
        }
    }
}