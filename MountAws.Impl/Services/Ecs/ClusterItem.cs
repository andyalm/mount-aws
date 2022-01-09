using Amazon.ECS.Model;
using MountAnything;

namespace MountAws.Services.Ecs;

public class ClusterItem : AwsItem<Cluster>
{
    public ClusterItem(ItemPath parentPath, Cluster cluster) : base(parentPath, cluster) {}

    public override string ItemName => UnderlyingObject.ClusterName;
    public override string ItemType => EcsItemTypes.Cluster;
    public override string? WebUrl => UrlBuilder.CombineWith($"ecs/home#/clusters/{ItemName}");
    public override bool IsContainer => true;
}