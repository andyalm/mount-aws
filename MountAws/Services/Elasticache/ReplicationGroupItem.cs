using Amazon.ElastiCache.Model;
using MountAnything;

namespace MountAws.Services.Elasticache;

public class ReplicationGroupItem : AwsItem<ReplicationGroup>
{
    public ReplicationGroupItem(ItemPath parentPath, ReplicationGroup replicationGroup) : base(parentPath, replicationGroup)
    {
        ItemName = replicationGroup.ReplicationGroupId;
        if (replicationGroup.NodeGroups.Count == 1)
        {
            PrimaryEndpoint = replicationGroup.NodeGroups[0].PrimaryEndpoint.ToAddressAndPortString();
            ReaderEndpoint = replicationGroup.NodeGroups[0].ReaderEndpoint.ToAddressAndPortString();
        }
    }

    public override string ItemName { get; }

    [ItemProperty] 
    public int MemberCount => UnderlyingObject.MemberClusters.Count;
    
    [ItemProperty]
    public string? PrimaryEndpoint { get; }
    
    [ItemProperty]
    public string? ReaderEndpoint { get; }
    
    public override bool IsContainer => true;
}