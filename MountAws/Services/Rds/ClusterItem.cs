using Amazon.RDS.Model;
using MountAnything;

namespace MountAws.Services.Rds;

public class ClusterItem : AwsItem<DBCluster>
{
    public ClusterItem(ItemPath parentPath, DBCluster cluster) : base(parentPath, cluster)
    {
        ItemName = cluster.DBClusterIdentifier;
    }

    [ItemProperty]
    public int MemberCount => UnderlyingObject.DBClusterMembers.Count;
    
    public override string ItemName { get; }
    
    public override bool IsContainer => true;
}