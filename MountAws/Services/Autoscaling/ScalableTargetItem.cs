using Amazon.ApplicationAutoScaling.Model;
using MountAnything;

namespace MountAws.Services.Autoscaling;

public class ScalableTargetItem(ItemPath parentPath, ScalableTarget underlyingObject)
    : AwsItem<ScalableTarget>(parentPath, underlyingObject)
{
    public override string ItemName => UnderlyingObject.
    public override bool IsContainer { get; }
}