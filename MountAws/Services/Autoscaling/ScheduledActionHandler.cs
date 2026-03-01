using Amazon.ApplicationAutoScaling;
using MountAnything;

namespace MountAws.Services.Autoscaling;

public class ScheduledActionHandler(ItemPath path, IPathHandlerContext context, CurrentServiceNamespace currentServiceNamespace, CurrentResourceId currentResourceId, IAmazonApplicationAutoScaling autoScaling)
    : PathHandler(path, context)
{
    protected override IItem? GetItemImpl()
    {
        var serviceNamespace = new ServiceNamespace(currentServiceNamespace.Value);
        var resourceId = currentResourceId.Value.Replace(":", "/");
        var action = autoScaling.DescribeScheduledActions(serviceNamespace, resourceId)
            .FirstOrDefault(a => a.ScheduledActionName == ItemName);

        return action != null ? new ScheduledActionItem(ParentPath, action) : null;
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return Enumerable.Empty<IItem>();
    }
}
