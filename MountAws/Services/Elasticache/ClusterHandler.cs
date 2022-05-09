using Amazon.ElastiCache;
using Amazon.ElastiCache.Model;
using MountAnything;

namespace MountAws.Services.Elasticache;

public class ClusterHandler : PathHandler
{
    private readonly IAmazonElastiCache _elastiCache;

    public ClusterHandler(ItemPath path, IPathHandlerContext context, IAmazonElastiCache elastiCache) : base(path, context)
    {
        _elastiCache = elastiCache;
    }

    protected override IItem? GetItemImpl()
    {
        try
        {
            var cacheCluster = _elastiCache.DescribeCacheCluster(ItemName);

            return new ClusterItem(ParentPath, cacheCluster);
        }
        catch (CacheClusterNotFoundException)
        {
            return null;
        }
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        try
        {
            var cacheCluster = _elastiCache.DescribeCacheCluster(ItemName, includeNodeInfo:true);

            return cacheCluster.CacheNodes.Select(cacheNode => new CacheNodeItem(Path, cacheNode));
        }
        catch (CacheClusterNotFoundException)
        {
            return Enumerable.Empty<IItem>();
        }
    }
}