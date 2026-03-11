using Amazon.ApplicationAutoScaling;
using MountAnything;

namespace MountAws.Services.AppAutoscaling;

public class ScalingPolicyHandler(
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
        var policy = autoScaling.DescribeScalingPolicies(serviceNamespace, resourceIdResolver.ResourceId)
            .FirstOrDefault(p => p.PolicyName == ItemName);

        return policy != null ? new ScalingPolicyItem(ParentPath, policy) : null;
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return Enumerable.Empty<IItem>();
    }
}
