using Amazon.ElastiCache.Model;
using MountAnything;

namespace MountAws.Services.Elasticache;

public class ClusterItem : AwsItem<CacheCluster>
{
    //TODO: Split this into a ReplicationGroupClusterItem and ClusterItem so we can provide a more optimized view for the each case
    public ClusterItem(ItemPath parentPath, CacheCluster cacheCluster) : base(parentPath, cacheCluster)
    {
        ItemName = cacheCluster.CacheClusterId;

        if (cacheCluster.CacheNodes?.Count == 1)
        {
            Endpoint = cacheCluster.CacheNodes[0].Endpoint.ToAddressAndPortString();
        }
    }

    public override string ItemName { get; }
    
    public override bool IsContainer => true;
    
    [ItemProperty]
    public string? Endpoint { get; }
}