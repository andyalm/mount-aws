using Amazon.ElasticLoadBalancingV2;

namespace MountAws.Services.ELBV2;

public class ListenerHandler : PathHandler
{
    private readonly IAmazonElasticLoadBalancingV2 _elbv2;

    public ListenerHandler(string path, IPathHandlerContext context, IAmazonElasticLoadBalancingV2 elbv2) : base(path, context)
    {
        _elbv2 = elbv2;
    }

    protected override bool ExistsImpl()
    {
        return GetItem() != null;
    }

    protected override AwsItem? GetItemImpl()
    {
        var loadBalancerHandler = new LoadBalancerHandler(ParentPath, Context, _elbv2);
        var loadBalancerItem = loadBalancerHandler.GetItem() as LoadBalancerItem;
        if (loadBalancerItem == null)
        {
            return null;
        }

        var listener = _elbv2.GetListeners(loadBalancerItem.LoadBalancer.LoadBalancerArn).SingleOrDefault(l => l.Port.ToString() == ItemName);
        if (listener != null)
        {
            return new ListenerItem(ParentPath, listener);
        }

        return null;
    }

    protected override IEnumerable<AwsItem> GetChildItemsImpl()
    {
        yield return DefaultActionsHandler.CreateItem(Path);
        yield return RulesHandler.CreateItem(Path);
    }
}