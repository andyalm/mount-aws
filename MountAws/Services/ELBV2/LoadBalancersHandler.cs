using Amazon.ElasticLoadBalancingV2;
using Amazon.ElasticLoadBalancingV2.Model;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.ELBV2;

public class LoadBalancersHandler : PathHandler
{
    private readonly IAmazonElasticLoadBalancingV2 _elbv2;

    public static Item CreateItem(string parentPath)
    {
        return new GenericContainerItem(parentPath, "load-balancers",
            "List and filter the load balancers within the current account and region");
    }
    
    public LoadBalancersHandler(string path, IPathHandlerContext context, IAmazonElasticLoadBalancingV2 elbv2) : base(path, context)
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
        var request = new DescribeLoadBalancersRequest();
        var response = _elbv2.DescribeLoadBalancersAsync(request).GetAwaiter().GetResult();
        return response.LoadBalancers.Select(lb => new LoadBalancerItem(Path, lb));
    }
}