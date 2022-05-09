using Amazon.ElastiCache.Model;
using MountAnything;

namespace MountAws.Services.Elasticache;

public class CacheNodeItem : AwsItem<CacheNode>
{
    public CacheNodeItem(ItemPath parentPath, CacheNode cacheNode) : base(parentPath, cacheNode)
    {
        ItemName = cacheNode.CacheNodeId;
        if (cacheNode.Endpoint != null)
        {
            EndpointAddress = $"{cacheNode.Endpoint.Address}:{cacheNode.Endpoint.Port}";
        }
    }

    public override string ItemName { get; }
    
    [ItemProperty]
    public string? EndpointAddress { get; }
    
    public override bool IsContainer => false;
}