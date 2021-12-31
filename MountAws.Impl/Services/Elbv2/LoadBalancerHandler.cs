using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.ElasticLoadBalancingV2;
using Amazon.ElasticLoadBalancingV2.Model;
using MountAnything;
using MountAws.Services.Ec2;

namespace MountAws.Services.Elbv2;

public class LoadBalancerHandler : PathHandler
{
    private readonly IAmazonElasticLoadBalancingV2 _elbv2;
    private readonly IAmazonEC2 _ec2;

    public LoadBalancerHandler(ItemPath path, IPathHandlerContext context, IAmazonElasticLoadBalancingV2 elbv2, IAmazonEC2 ec2) : base(path, context)
    {
        _elbv2 = elbv2;
        _ec2 = ec2;
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
            var securityGroups = _ec2.DescribeSecurityGroups(new DescribeSecurityGroupsRequest
            {
                GroupIds = loadBalancer.SecurityGroups
            });
            return new LoadBalancerItem(ParentPath, loadBalancer, securityGroups);
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