using Amazon.EC2;
using Amazon.ElasticLoadBalancingV2;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Elbv2;

public class RulesHandler : PathHandler
{
    private readonly IAmazonElasticLoadBalancingV2 _elbv2;
    private readonly IAmazonEC2 _ec2;

    public static Item CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "rules",
            "List the rules attached to the load balancer listener");
    }
    
    public RulesHandler(ItemPath path, IPathHandlerContext context, IAmazonElasticLoadBalancingV2 elbv2, IAmazonEC2 ec2) : base(path, context)
    {
        _elbv2 = elbv2;
        _ec2 = ec2;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        var listenerHandler = new ListenerHandler(ParentPath, Context, _elbv2, _ec2);
        var listener = listenerHandler.GetItem() as ListenerItem;
        if (listener == null)
        {
            return Enumerable.Empty<Item>();
        }

        return _elbv2.DescribeRules(listener.ListenerArn).Select(r => new RuleItem(Path, r));
    }
}