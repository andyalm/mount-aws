using Amazon.ApplicationAutoScaling.Model;
using MountAnything;

namespace MountAws.Services.Autoscaling;

public class ScalableTargetItem(ItemPath parentPath, ScalableTarget underlyingObject)
    : AwsItem<ScalableTarget>(parentPath, underlyingObject)
{
    public override string ItemName => UnderlyingObject.ResourceId.Replace("/", ":");
    public override bool IsContainer => true;

    [ItemProperty]
    public string ScalableDimension => UnderlyingObject.ScalableDimension.Value;

    [ItemProperty]
    public int MinCapacity => UnderlyingObject.MinCapacity;

    [ItemProperty]
    public int MaxCapacity => UnderlyingObject.MaxCapacity;

    [ItemProperty]
    public string ServiceNamespace => UnderlyingObject.ServiceNamespace.Value;
}
