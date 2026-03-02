using Amazon.ApplicationAutoScaling;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.AppAutoscaling;

public class ScheduledActionsHandler(
    ItemPath path,
    IPathHandlerContext context,
    CurrentServiceNamespace currentServiceNamespace,
    IResourceIdResolver resourceIdResolver,
    IAmazonApplicationAutoScaling autoScaling)
    : PathHandler(path, context)
{
    public static Item CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "scheduled-actions",
            "Navigate the scheduled actions for this scalable target");
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        var serviceNamespace = new ServiceNamespace(currentServiceNamespace.Value);
        return autoScaling.DescribeScheduledActions(serviceNamespace, resourceIdResolver.ResourceId)
            .Select(a => new ScheduledActionItem(Path, a));
    }
}
