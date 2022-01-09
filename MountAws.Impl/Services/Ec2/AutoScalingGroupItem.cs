using Amazon.AutoScaling.Model;
using MountAnything;

namespace MountAws.Services.Ec2;

public class AutoScalingGroupItem : AwsItem<AutoScalingGroup>
{
    public AutoScalingGroupItem(ItemPath parentPath, AutoScalingGroup underlyingObject) : base(parentPath, underlyingObject)
    {
        ItemName = underlyingObject.AutoScalingGroupName;
    }

    public override string ItemName { get; }
    public override bool IsContainer => true;

    [ItemProperty]
    public int? NumInstances => UnderlyingObject.Instances?.Count;

    [ItemProperty]
    public IEnumerable<string> InstanceTypes => UnderlyingObject.Instances.Select(i => i.InstanceType).Distinct();
}