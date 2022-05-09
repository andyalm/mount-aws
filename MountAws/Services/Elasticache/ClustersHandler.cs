using Amazon.ElastiCache;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Elasticache;

public class ClustersHandler : PathHandler
{
    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "clusters",
            "Navigate elasticache memcached and single-node redis clusters");
    }
    
    private readonly IAmazonElastiCache _elastiCache;

    public ClustersHandler(ItemPath path, IPathHandlerContext context, IAmazonElastiCache elastiCache) : base(path, context)
    {
        _elastiCache = elastiCache;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return _elastiCache.DescribeCacheClusters(replicationGroups:false).Select(cacheCluster => new ClusterItem(Path, cacheCluster));
    }
}