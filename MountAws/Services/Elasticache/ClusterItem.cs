using Amazon.ElastiCache.Model;
using MountAnything;

namespace MountAws.Services.Elasticache;

public class ClusterItem : AwsItem<CacheCluster>
{
    public ClusterItem(ItemPath parentPath, CacheCluster cacheCluster) : base(parentPath, cacheCluster)
    {
        ItemName = cacheCluster.CacheClusterId;
    }

    public override string ItemName { get; }
    
    public override bool IsContainer => true;
}