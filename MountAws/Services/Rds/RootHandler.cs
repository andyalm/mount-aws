using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Rds;

public class RootHandler : PathHandler
{
    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "rds",
            "Navigate RDS objects in the current account and region");
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
        yield return ClustersHandler.CreateItem(Path);
        yield return InstancesHandler.CreateItem(Path);
    }
}