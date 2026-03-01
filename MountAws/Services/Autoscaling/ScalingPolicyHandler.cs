using Amazon.ApplicationAutoScaling;
using MountAnything;

namespace MountAws.Services.Autoscaling;

public class ScalingPolicyHandler(ItemPath path, IPathHandlerContext context, CurrentServiceNamespace currentServiceNamespace, CurrentResourceId currentResourceId, IAmazonApplicationAutoScaling autoScaling)
    : PathHandler(path, context)
{
    protected override IItem? GetItemImpl()
    {
        var serviceNamespace = new ServiceNamespace(currentServiceNamespace.Value);
        var resourceId = currentResourceId.Value.Replace(":", "/");
        var policy = autoScaling.DescribeScalingPolicies(serviceNamespace, resourceId)
            .FirstOrDefault(p => p.PolicyName == ItemName);

        return policy != null ? new ScalingPolicyItem(ParentPath, policy) : null;
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return Enumerable.Empty<IItem>();
    }
}
