using Amazon.ElasticLoadBalancingV2;
using Amazon.ElasticLoadBalancingV2.Model;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.ELBV2;

public class RulesHandler : PathHandler
{
    private readonly IAmazonElasticLoadBalancingV2 _elbv2;

    public static Item CreateItem(string parentPath)
    {
        return new GenericContainerItem(parentPath, "rules",
            "List the rules attached to the load balancer listener");
    }
    
    public RulesHandler(string path, IPathHandlerContext context, IAmazonElasticLoadBalancingV2 elbv2) : base(path, context)
    {
        _elbv2 = elbv2;
    }

    protected override bool ExistsImpl()
    {
        return true;
    }

    protected override Item? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<Item> GetChildItemsImpl()
    {
        var listenerHandler = new ListenerHandler(ParentPath, Context, _elbv2);
        var listener = listenerHandler.GetItem() as ListenerItem;
        if (listener == null)
        {
            return Enumerable.Empty<Item>();
        }

        var response = _elbv2.DescribeRulesAsync(new DescribeRulesRequest
        {
            ListenerArn = listener.Listener.ListenerArn
        });

        return response.GetAwaiter().GetResult().Rules.Select(r => new RuleItem(Path, r));
    }
}