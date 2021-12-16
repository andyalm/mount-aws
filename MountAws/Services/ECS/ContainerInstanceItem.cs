using Amazon.ECS.Model;
using MountAnything;

namespace MountAws.Services.ECS;

public class ContainerInstanceItem : Item
{
    private readonly ContainerInstance _containerInstance;

    public ContainerInstanceItem(string parentPath, ContainerInstance containerInstance) : base(parentPath)
    {
        _containerInstance = containerInstance;
        ItemName = containerInstance.ContainerInstanceArn.Split("/").Last();
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