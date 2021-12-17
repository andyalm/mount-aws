using System.Collections.Immutable;
using System.Management.Automation;
using Amazon.ECS.Model;
using MountAnything;
using MountAws.Services.EC2;

namespace MountAws.Services.ECS;

public class ContainerInstanceItem : Item
{
    private readonly ContainerInstance _containerInstance;

    public ContainerInstanceItem(string parentPath, ContainerInstance containerInstance, EC2InstanceItem? ec2Instance) : base(parentPath)
    {
        _containerInstance = containerInstance;
        ItemName = containerInstance.ContainerInstanceArn.Split("/").Last();
        if (ec2Instance != null)
        {
            Links = ImmutableDictionary.Create<string,Item>().Add("Ec2Instance", ec2Instance);
        }
    }

    public override string ItemName { get; }
    public override object UnderlyingObject => _containerInstance;
    public override string ItemType => "ContainerInstance";
    public override bool IsContainer => true;

    public override IEnumerable<string> Aliases
    {
        get
        {
            yield return _containerInstance.Ec2InstanceId;
        }
    }
}