using Amazon.RDS;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Rds;

public class ClustersHandler : PathHandler
{
    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "clusters",
            "Navigate RDS DB Clusters (i.e. Aurora) in the current account and region");
    }
    
    private readonly IAmazonRDS _rds;

    public ClustersHandler(ItemPath path, IPathHandlerContext context, IAmazonRDS rds) : base(path, context)
    {
        _rds = rds;
    }

    protected override IItem GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return _rds.DescribeDBClusters()
            .Select(cluster => new ClusterItem(Path, cluster));
    }
}