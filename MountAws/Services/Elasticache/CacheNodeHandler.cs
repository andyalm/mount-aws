using Amazon.ElastiCache;
using MountAnything;

namespace MountAws.Services.Elasticache;

public class CacheNodeHandler : PathHandler
{
    private readonly ClusterHandler _parentHandler;
    
    public CacheNodeHandler(ItemPath path, IPathHandlerContext context, IAmazonElastiCache elastiCache) : base(path, context)
    {
        _parentHandler = new ClusterHandler(path.Parent, context, elastiCache);
    }

    protected override IItem? GetItemImpl()
    {
        return _parentHandler.GetChildItems()
            .SingleOrDefault(i => i.ItemName.Equals(ItemName, StringComparison.OrdinalIgnoreCase));
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield break;
    }
}