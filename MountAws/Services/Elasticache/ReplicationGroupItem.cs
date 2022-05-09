using Amazon.ElastiCache.Model;
using MountAnything;

namespace MountAws.Services.Elasticache;

public class ReplicationGroupItem : AwsItem<ReplicationGroup>
{
    public ReplicationGroupItem(ItemPath parentPath, ReplicationGroup replicationGroup) : base(parentPath, replicationGroup)
    {
        ItemName = replicationGroup.ReplicationGroupId;
    }

    public override string ItemName { get; }

    [ItemProperty] 
    public int MemberCount => UnderlyingObject.MemberClusters.Count;
    
    public override bool IsContainer => true;
}