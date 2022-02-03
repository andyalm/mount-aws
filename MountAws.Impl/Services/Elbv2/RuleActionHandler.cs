using Amazon.EC2;
using Amazon.ElasticLoadBalancingV2;
using MountAnything;

namespace MountAws.Services.Elbv2;

public class RuleActionHandler : PathHandler
{
    private readonly IAmazonElasticLoadBalancingV2 _elbv2;
    private readonly IAmazonEC2 _ec2;

    public RuleActionHandler(ItemPath path, IPathHandlerContext context, IAmazonElasticLoadBalancingV2 elbv2, IAmazonEC2 ec2) : base(path, context)
    {
        _elbv2 = elbv2;
        _ec2 = ec2;
    }

    protected override IItem? GetItemImpl()
    {
        var ruleHandler = new RuleHandler(ParentPath, Context, _elbv2, _ec2);
        return ruleHandler.GetChildItems().SingleOrDefault(r => r.ItemName == ItemName);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        if (GetItem() is ActionItem item)
        {
            return item.GetChildren(_elbv2, LinkGenerator);
        }
        
        return Enumerable.Empty<Item>();
    }
}