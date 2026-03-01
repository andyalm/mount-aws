using Amazon.ApplicationAutoScaling.Model;
using MountAnything;

namespace MountAws.Services.Autoscaling;

public class ScalingPolicyItem(ItemPath parentPath, ScalingPolicy underlyingObject)
    : AwsItem<ScalingPolicy>(parentPath, underlyingObject)
{
    public override string ItemName => UnderlyingObject.PolicyName;
    public override bool IsContainer => false;

    [ItemProperty]
    public string PolicyType => UnderlyingObject.PolicyType;

    [ItemProperty]
    public string ScalableDimension => UnderlyingObject.ScalableDimension.Value;

    [ItemProperty]
    public string ResourceId => UnderlyingObject.ResourceId;

    [ItemProperty]
    public DateTime CreationTime => UnderlyingObject.CreationTime;
}
