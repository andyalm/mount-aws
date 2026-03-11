using Amazon.ApplicationAutoScaling;
using MountAnything;

namespace MountAws.Services.AppAutoscaling;

public class ScalingActivityHandler(
    ItemPath path,
    IPathHandlerContext context,
    CurrentServiceNamespace currentServiceNamespace,
    IResourceIdResolver resourceIdResolver,
    IAmazonApplicationAutoScaling autoScaling) : PathHandler(path, context)
{
    protected override IItem? GetItemImpl()
    {
        var scalingActivity = autoScaling.DescribeScalingActivities(currentServiceNamespace.Value, resourceIdResolver.ResourceId)
            .FirstOrDefault(a => a.ActivityId == ItemName);
        
        return scalingActivity == null ? null : new ScalingActivityItem(ParentPath, scalingActivity);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield break;
    }
}