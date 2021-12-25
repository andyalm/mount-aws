using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Ecs;

public class ECSRootHandler : PathHandler
{
    public static Item CreateItem(string parentPath)
    {
        return new GenericContainerItem(parentPath, "ecs",
            "Navigate ECS clusters, services, tasks, etc");
    }

    public ECSRootHandler(string path, IPathHandlerContext context) : base(path, context)
    {
        
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield return ClustersHandler.CreateItem(Path);
    }
}