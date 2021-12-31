using Amazon.EC2;
using Amazon.ElasticLoadBalancingV2;
using MountAnything;

namespace MountAws.Services.Elbv2;

public class DefaultActionHandler : PathHandler
{
    private readonly IAmazonElasticLoadBalancingV2 _elbv2;
    private readonly IAmazonEC2 _ec2;

    public DefaultActionHandler(ItemPath path, IPathHandlerContext context, IAmazonElasticLoadBalancingV2 elbv2, IAmazonEC2 ec2) : base(path, context)
    {
        _elbv2 = elbv2;
        _ec2 = ec2;
    }

    protected override IItem? GetItemImpl()
    {
        var defaultActionsHandler = new DefaultActionsHandler(ParentPath, Context, _elbv2, _ec2);
        return defaultActionsHandler
            .GetChildItems()
            .SingleOrDefault(i => i.ItemName.Equals(ItemName, StringComparison.OrdinalIgnoreCase));
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        if (GetItem() is ActionItem item)
        {
            return item.GetChildren(_elbv2);
        }
        
        return Enumerable.Empty<Item>();
    }
}