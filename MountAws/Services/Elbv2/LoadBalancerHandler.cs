using MountAnything;
using MountAws.Api.Elbv2;
using LoadBalancerNotFoundException = MountAws.Api.Elbv2.LoadBalancerNotFoundException;

namespace MountAws.Services.Elbv2;

public class LoadBalancerHandler : PathHandler
{
    private readonly IElbv2Api _elbv2;

    public LoadBalancerHandler(string path, IPathHandlerContext context, IElbv2Api elbv2) : base(path, context)
    {
        _elbv2 = elbv2;
    }

    protected override bool ExistsImpl()
    {
        return GetItem() != null;
    }

    protected override IItem? GetItemImpl()
    {
        try
        {
            var loadBalancer = _elbv2.DescribeLoadBalancer(ItemName);
            return new LoadBalancerItem(ParentPath, loadBalancer);
        }
        catch (LoadBalancerNotFoundException)
        {
            return null;
        }
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        var loadBalancerItem = GetItem() as LoadBalancerItem;
        if (loadBalancerItem == null)
        {
            return Enumerable.Empty<Item>();
        }
        
        return _elbv2.DescribeListeners(loadBalancerItem.LoadBalancerArn)
            .Select(l => new ListenerItem(Path, l))
            .OrderBy(l => l.Port);
    }
}