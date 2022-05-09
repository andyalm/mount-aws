using Amazon.ElastiCache;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Elasticache;

public class RootHandler : PathHandler
{
    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "elasticache",
            "Navigate the elasticache api as a hierarchy of objects");
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
        yield return ReplicationGroupsHandler.CreateItem(Path);
    }
}