using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.AppAutoscaling;

public class AutoscalingHandler(ItemPath path, IPathHandlerContext context) : PathHandler(path, context)
{
    public static Item CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "autoscaling",
            "Navigate application autoscaling dimensions");
    }
    
    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield return ScalingPoliciesHandler.CreateItem(Path);
        yield return ScalingActivitiesHandler.CreateItem(Path);
        yield return ScheduledActionsHandler.CreateItem(Path);
    }
}