using Amazon.ElastiCache;
using Amazon.ElastiCache.Model;

using static MountAws.PagingHelper;

namespace MountAws.Services.Elasticache;

public static class ApiExtensions
{
    public static IEnumerable<CacheCluster> DescribeCacheClusters(this IAmazonElastiCache elastiCache, bool replicationGroups)
    {
        return Paginate(nextToken =>
        {
            var response = elastiCache.DescribeCacheClustersAsync(new DescribeCacheClustersRequest
            {
                Marker = nextToken,
                ShowCacheClustersNotInReplicationGroups = !replicationGroups,
                ShowCacheNodeInfo = true
            }).GetAwaiter().GetResult();

            return (response.CacheClusters, response.Marker);
        });
    }

    public static CacheCluster DescribeCacheCluster(this IAmazonElastiCache elastiCache, string id)
    {
        return elastiCache.DescribeCacheClustersAsync(new DescribeCacheClustersRequest
        {
            CacheClusterId = id,
            ShowCacheNodeInfo = true
        }).GetAwaiter().GetResult().CacheClusters.Single();
    }

    public static IEnumerable<ReplicationGroup> DescribeReplicationGroups(this IAmazonElastiCache elastiCache)
    {
        return Paginate(nextToken =>
        {
            var response = elastiCache.DescribeReplicationGroupsAsync(new DescribeReplicationGroupsRequest
            {
                Marker = nextToken
            }).GetAwaiter().GetResult();

            return (response.ReplicationGroups, response.Marker);
        });
    }

    public static ReplicationGroup DescribeReplicationGroup(this IAmazonElastiCache elastiCache, string id)
    {
        return elastiCache.DescribeReplicationGroupsAsync(new DescribeReplicationGroupsRequest
        {
            ReplicationGroupId = id
        }).GetAwaiter().GetResult().ReplicationGroups.Single();
    }
}