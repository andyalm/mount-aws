using Amazon.ApplicationAutoScaling;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Autoscaling;

public class ScheduledActionsHandler(ItemPath path, IPathHandlerContext context, CurrentServiceNamespace currentServiceNamespace, CurrentResourceId currentResourceId, IAmazonApplicationAutoScaling autoScaling)
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
        var resourceId = currentResourceId.Value.Replace(":", "/");
        return autoScaling.DescribeScheduledActions(serviceNamespace, resourceId)
            .Select(a => new ScheduledActionItem(Path, a));
    }
}
