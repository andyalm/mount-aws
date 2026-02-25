using Amazon.ApplicationAutoScaling;
using Amazon.ApplicationAutoScaling.Model;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Autoscaling;

public class ScalableTargetsHandler(ItemPath path, IPathHandlerContext context, ServiceNamespace serviceNamespace, IAmazonApplicationAutoScaling autoScaling)
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
        autoScaling.DescribeScalableTargets(serviceNamespace).Select(target => new ScalableTargetItem)
    }
}