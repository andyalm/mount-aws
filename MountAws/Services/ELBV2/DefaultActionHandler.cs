using Amazon.ElasticLoadBalancingV2;

namespace MountAws.Services.ELBV2;

public class DefaultActionHandler : PathHandler
{
    private readonly IAmazonElasticLoadBalancingV2 _elbv2;

    public DefaultActionHandler(string path, IPathHandlerContext context, IAmazonElasticLoadBalancingV2 elbv2) : base(path, context)
    {
        _elbv2 = elbv2;
    }

    protected override bool ExistsImpl()
    {
        return GetItem() != null;
    }

    protected override AwsItem? GetItemImpl()
    {
        var defaultActionsHandler = new DefaultActionsHandler(ParentPath, Context, _elbv2);
        return defaultActionsHandler
            .GetChildItems()
            .SingleOrDefault(i => i.ItemName.Equals(ItemName, StringComparison.OrdinalIgnoreCase));
    }

    protected override IEnumerable<AwsItem> GetChildItemsImpl()
    {
        if (GetItem() is ActionItem item)
        {
            return item.GetChildren(_elbv2);
        }
        
        return Enumerable.Empty<AwsItem>();
    }
}