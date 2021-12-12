using Amazon.ElasticLoadBalancingV2;
using Amazon.ElasticLoadBalancingV2.Model;

namespace MountAws.Services.ELBV2;

public class LoadBalancerHandler : PathHandler
{
    private readonly IAmazonElasticLoadBalancingV2 _elbv2;

    public LoadBalancerHandler(string path, IPathHandlerContext context, IAmazonElasticLoadBalancingV2 elbv2) : base(path, context)
    {
        _elbv2 = elbv2;
    }

    protected override bool ExistsImpl()
    {
        return GetItem() != null;
    }

    protected override AwsItem? GetItemImpl()
    {
        var loadBalancer = _elbv2.GetLoadBalancer(ItemName);
        if (loadBalancer != null)
        {
            return new LoadBalancerItem(ParentPath, loadBalancer);
        }
        
        return null;
    }

    protected override IEnumerable<AwsItem> GetChildItemsImpl()
    {
        var loadBalancer = ((LoadBalancerItem)GetItem()!).LoadBalancer;
        var request = new DescribeListenersRequest
        {
            LoadBalancerArn = loadBalancer.LoadBalancerArn
        };
        var response = _elbv2.DescribeListenersAsync(request).GetAwaiter().GetResult();

        return response.Listeners
            .Select(l => new ListenerItem(Path, l))
            .OrderBy(l => l.Listener.Port);
    }
}