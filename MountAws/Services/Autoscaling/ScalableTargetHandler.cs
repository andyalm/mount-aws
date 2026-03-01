using Amazon.ApplicationAutoScaling;
using Amazon.ApplicationAutoScaling.Model;
using MountAnything;

namespace MountAws.Services.Autoscaling;

public class ScalableTargetHandler(ItemPath path, IPathHandlerContext context, CurrentServiceNamespace currentServiceNamespace, CurrentResourceId currentResourceId, IAmazonApplicationAutoScaling autoScaling)
    : PathHandler(path, context)
{
    protected override IItem? GetItemImpl()
    {
        var serviceNamespace = new ServiceNamespace(currentServiceNamespace.Value);
        var resourceId = currentResourceId.Value.Replace(":", "/");

        var target = autoScaling.DescribeScalableTargets(serviceNamespace)
            .FirstOrDefault(t => t.ResourceId == resourceId);

        return target != null ? new ScalableTargetItem(ParentPath, target) : null;
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield return ScalingPoliciesHandler.CreateItem(Path);
        yield return ScalingActivitiesHandler.CreateItem(Path);
        yield return ScheduledActionsHandler.CreateItem(Path);
    }
}
