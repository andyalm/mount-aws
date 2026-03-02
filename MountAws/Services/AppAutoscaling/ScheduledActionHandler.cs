using Amazon.ApplicationAutoScaling;
using MountAnything;

namespace MountAws.Services.AppAutoscaling;

public class ScheduledActionHandler(
    ItemPath path,
    IPathHandlerContext context, 
    CurrentServiceNamespace currentServiceNamespace,
    IResourceIdResolver resourceIdResolver,
    IAmazonApplicationAutoScaling autoScaling)
    : PathHandler(path, context)
{
    protected override IItem? GetItemImpl()
    {
        var serviceNamespace = new ServiceNamespace(currentServiceNamespace.Value);
        var action = autoScaling.DescribeScheduledActions(serviceNamespace, resourceIdResolver.ResourceId)
            .FirstOrDefault(a => a.ScheduledActionName == ItemName);

        return action != null ? new ScheduledActionItem(ParentPath, action) : null;
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return Enumerable.Empty<IItem>();
    }
}
