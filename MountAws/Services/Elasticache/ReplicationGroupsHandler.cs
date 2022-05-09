using Amazon.ElastiCache;
using Amazon.ElastiCache.Model;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Elasticache;

public class ReplicationGroupsHandler : PathHandler
{
    private readonly IAmazonElastiCache _elastiCache;

    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "replication-groups",
            "Navigate the replication groups");
    }
    
    public ReplicationGroupsHandler(ItemPath path, IPathHandlerContext context, IAmazonElastiCache elastiCache) : base(path, context)
    {
        _elastiCache = elastiCache;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return _elastiCache.DescribeReplicationGroups()
            .Select(g => new ReplicationGroupItem(Path, g));
    }
}