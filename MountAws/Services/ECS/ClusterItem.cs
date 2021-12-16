using Amazon.ECS.Model;
using MountAnything;

namespace MountAws.Services.ECS;

public class ClusterItem : Item
{
    private readonly Cluster _cluster;

    public ClusterItem(string parentPath, Cluster cluster) : base(parentPath)
    {
        _cluster = cluster;
    }

    public override string ItemName => _cluster.ClusterName;
    public override object UnderlyingObject => _cluster;
    public override string ItemType => "Cluster";
    public override bool IsContainer => true;
}