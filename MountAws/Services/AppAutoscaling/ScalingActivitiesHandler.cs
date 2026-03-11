using Amazon.ApplicationAutoScaling;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.AppAutoscaling;

public class ScalingActivitiesHandler(
    ItemPath path,
    IPathHandlerContext context,
    CurrentServiceNamespace currentServiceNamespace,
    IResourceIdResolver resourceIdResolver,
    IAmazonApplicationAutoScaling autoScaling)
    : PathHandler(path, context)
{
    public static Item CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "scaling-activities",
            "Navigate the scaling activities for this scalable target");
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        var serviceNamespace = new ServiceNamespace(currentServiceNamespace.Value);
        return autoScaling.DescribeScalingActivities(serviceNamespace, resourceIdResolver.ResourceId)
            .Select(a => new ScalingActivityItem(Path, a));
    }
}
