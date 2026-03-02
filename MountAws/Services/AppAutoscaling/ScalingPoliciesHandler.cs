using Amazon.ApplicationAutoScaling;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.AppAutoscaling;

public class ScalingPoliciesHandler(
    ItemPath path,
    IPathHandlerContext context, 
    CurrentServiceNamespace currentServiceNamespace,
    IResourceIdResolver resourceIdResolver,
    IAmazonApplicationAutoScaling autoScaling)
    : PathHandler(path, context)
{
    public static Item CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "scaling-policies",
            "Navigate the scaling policies for this scalable target");
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        var serviceNamespace = new ServiceNamespace(currentServiceNamespace.Value);
        return autoScaling.DescribeScalingPolicies(serviceNamespace, resourceIdResolver.ResourceId)
            .Select(p => new ScalingPolicyItem(Path, p));
    }
}
