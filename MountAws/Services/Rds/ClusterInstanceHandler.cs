using Amazon.RDS;
using MountAnything;

namespace MountAws.Services.Rds;

public class ClusterInstanceHandler : PathHandler
{
    private readonly IAmazonRDS _rds;

    public ClusterInstanceHandler(ItemPath path, IPathHandlerContext context, IAmazonRDS rds) : base(path, context)
    {
        _rds = rds;
    }

    protected override IItem? GetItemImpl()
    {
        var instance = _rds.DescribeDBInstance(ItemName);
        
        return instance != null ? new InstanceItem(ParentPath, instance) : null;
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield break;
    }
}