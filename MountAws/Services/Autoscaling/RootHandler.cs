using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Autoscaling;

public class RootHandler : PathHandler
{
    public static Item CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "autoscaling",
            "Navigate application autoscaling dimensions");
    }

    public RootHandler(ItemPath path, IPathHandlerContext context) : base(path, context)
    {
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield return ServicesHandler.CreateItem(Path);
    }
}
