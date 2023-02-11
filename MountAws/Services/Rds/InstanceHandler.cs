using Amazon.RDS;
using MountAnything;

namespace MountAws.Services.Rds;

public class InstanceHandler : PathHandler
{
    private readonly IAmazonRDS _rds;

    public InstanceHandler(ItemPath path, IPathHandlerContext context, IAmazonRDS rds) : base(path, context)
    {
        _rds = rds;
    }

    protected override IItem? GetItemImpl()
    {
        var dbInstance = _rds.DescribeDBInstance(ItemName);
        
        return dbInstance != null ? new DbInstanceItem(Path, dbInstance) : null;
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield break;
    }
}