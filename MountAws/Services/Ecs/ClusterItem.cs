using System.Management.Automation;
using MountAnything;

namespace MountAws.Services.Ecs;

public class ClusterItem : Item
{
    public ClusterItem(string parentPath, PSObject cluster) : base(parentPath, cluster) {}

    public override string ItemName => Property<string>("ClusterName")!;
    public override string ItemType => EcsItemTypes.Cluster;
    public override bool IsContainer => true;
}