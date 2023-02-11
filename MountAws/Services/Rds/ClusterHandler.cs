using Amazon.RDS;
using Amazon.RDS.Model;
using MountAnything;

namespace MountAws.Services.Rds;

public class ClusterHandler : PathHandler
{
    private readonly IAmazonRDS _rds;

    public ClusterHandler(ItemPath path, IPathHandlerContext context, IAmazonRDS rds) : base(path, context)
    {
        _rds = rds;
    }

    protected override IItem? GetItemImpl()
    {
        var cluster = _rds.DescribeDBCluster(ItemName);
        
        return cluster != null ? new ClusterItem(ParentPath, cluster) : null;
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        var clusterItem = GetItem() as ClusterItem;
        if (clusterItem == null)
            return Enumerable.Empty<IItem>();

        return _rds.DescribeDBInstances(new Filter
        {
            Name = "db-cluster-id",
            Values = new List<string>{clusterItem.ItemName}
        }).Select(db => new InstanceItem(Path, db));
    }
}