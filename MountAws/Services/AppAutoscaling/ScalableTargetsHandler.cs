using Amazon.ApplicationAutoScaling;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.AppAutoscaling;

public class ScalableTargetsHandler(ItemPath path, IPathHandlerContext context, CurrentServiceNamespace currentServiceNamespace, IAmazonApplicationAutoScaling autoScaling)
    : PathHandler(path, context)
{
    public static Item CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "scalable-targets",
            "Navigate the scalable targets of the current service namespace.");
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return autoScaling.DescribeScalableTargets(currentServiceNamespace.Value)
            .Select(target => new ScalableTargetItem(Path, target));
    }
}
