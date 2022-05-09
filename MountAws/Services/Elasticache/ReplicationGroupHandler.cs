using Amazon.ElastiCache;
using Amazon.ElastiCache.Model;
using MountAnything;

namespace MountAws.Services.Elasticache;

public class ReplicationGroupHandler : PathHandler
{
    private readonly IAmazonElastiCache _elastiCache;

    public ReplicationGroupHandler(ItemPath path, IPathHandlerContext context, IAmazonElastiCache elastiCache) : base(path, context)
    {
        _elastiCache = elastiCache;
    }

    protected override IItem? GetItemImpl()
    {
        try
        {
            var replicationGroup = _elastiCache.DescribeReplicationGroup(ItemName);

            return new ReplicationGroupItem(ParentPath, replicationGroup);
        }
        catch (ReplicationGroupNotFoundException)
        {
            return null;
        }
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return _elastiCache.DescribeCacheClusters(replicationGroups: true)
            .Where(c => c.ReplicationGroupId?.Equals(ItemName,
                StringComparison.OrdinalIgnoreCase) == true)
            .Select(c => new ClusterItem(Path, c));
    }
}