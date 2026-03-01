using Amazon.ApplicationAutoScaling.Model;
using MountAnything;

namespace MountAws.Services.Autoscaling;

public class ScheduledActionItem(ItemPath parentPath, ScheduledAction underlyingObject)
    : AwsItem<ScheduledAction>(parentPath, underlyingObject)
{
    public override string ItemName => UnderlyingObject.ScheduledActionName;
    public override bool IsContainer => false;

    [ItemProperty]
    public string Schedule => UnderlyingObject.Schedule;

    [ItemProperty]
    public string ResourceId => UnderlyingObject.ResourceId;

    [ItemProperty]
    public string ScalableDimension => UnderlyingObject.ScalableDimension.Value;

    [ItemProperty]
    public DateTime? StartTime => UnderlyingObject.StartTime;

    [ItemProperty]
    public DateTime? EndTime => UnderlyingObject.EndTime;

    [ItemProperty]
    public DateTime CreationTime => UnderlyingObject.CreationTime;
}
