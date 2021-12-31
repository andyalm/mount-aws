using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.ElasticLoadBalancingV2;
using MountAnything;
using MountAws.Services.Core;
using MountAws.Services.Ec2;

namespace MountAws.Services.Elbv2;

public class LoadBalancersHandler : PathHandler
{
    private readonly IAmazonElasticLoadBalancingV2 _elbv2;
    private readonly IAmazonEC2 _ec2;

    public static Item CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "load-balancers",
            "List and filter the load balancers within the current account and region");
    }
    
    public LoadBalancersHandler(ItemPath path, IPathHandlerContext context, IAmazonElasticLoadBalancingV2 elbv2, IAmazonEC2 ec2) : base(path, context)
    {
        _elbv2 = elbv2;
        _ec2 = ec2;
    }

    protected override bool ExistsImpl()
    {
        return true;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        var loadBalancers = _elbv2.DescribeLoadBalancers();

        var securityGroupIds = loadBalancers
                .SelectMany(lb => lb.SecurityGroups ?? Enumerable.Empty<string>())
                .Distinct()
                .ToList();

        var securityGroups = _ec2.DescribeSecurityGroups(new DescribeSecurityGroupsRequest
            {
                GroupIds = securityGroupIds
            }).ToDictionary(sg => sg.GroupId);

        return loadBalancers.Select(lb => new LoadBalancerItem(Path, lb, securityGroups.MultiGet(lb.SecurityGroups)))
            .OrderBy(lb => lb.ItemName);
    }
}