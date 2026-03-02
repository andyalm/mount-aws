using Amazon.ApplicationAutoScaling.Model;
using MountAnything;

namespace MountAws.Services.AppAutoscaling;

public class ScalingActivityItem(ItemPath parentPath, ScalingActivity underlyingObject)
    : AwsItem<ScalingActivity>(parentPath, underlyingObject)
{
    public override string ItemName => UnderlyingObject.ActivityId;
    public override bool IsContainer => false;

    [ItemProperty]
    public string StatusCode => UnderlyingObject.StatusCode.Value;

    [ItemProperty]
    public string Description => UnderlyingObject.Description;

    [ItemProperty]
    public string Cause => UnderlyingObject.Cause;

    [ItemProperty]
    public DateTime StartTime => UnderlyingObject.StartTime;

    [ItemProperty]
    public DateTime? EndTime => UnderlyingObject.EndTime;

    [ItemProperty]
    public string ResourceId => UnderlyingObject.ResourceId;

    [ItemProperty]
    public string ScalableDimension => UnderlyingObject.ScalableDimension.Value;
}
