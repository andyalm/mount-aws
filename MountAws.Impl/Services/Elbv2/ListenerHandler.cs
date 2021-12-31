using Amazon.EC2;
using Amazon.ElasticLoadBalancingV2;
using MountAnything;

namespace MountAws.Services.Elbv2;

public class ListenerHandler : PathHandler
{
    private readonly IAmazonElasticLoadBalancingV2 _elbv2;
    private readonly IAmazonEC2 _ec2;

    public ListenerHandler(ItemPath path, IPathHandlerContext context, IAmazonElasticLoadBalancingV2 elbv2, IAmazonEC2 ec2) : base(path, context)
    {
        _elbv2 = elbv2;
        _ec2 = ec2;
    }

    protected override IItem? GetItemImpl()
    {
        var loadBalancerHandler = new LoadBalancerHandler(ParentPath, Context, _elbv2, _ec2);
        var loadBalancerItem = loadBalancerHandler.GetItem() as LoadBalancerItem;
        if (loadBalancerItem == null)
        {
            return null;
        }

        var listener = _elbv2.DescribeListeners(loadBalancerItem.LoadBalancerArn)
            .SingleOrDefault(l => l.Port.ToString() == ItemName);
        if (listener != null)
        {
            return new ListenerItem(ParentPath, listener);
        }

        return null;
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield return DefaultActionsHandler.CreateItem(Path);
        yield return RulesHandler.CreateItem(Path);
    }
}