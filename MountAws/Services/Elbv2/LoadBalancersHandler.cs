using System.Management.Automation;
using MountAnything;
using MountAws.Api.Ec2;
using MountAws.Api.Elbv2;
using MountAws.Services.Core;

using static MountAws.PagingHelper;

namespace MountAws.Services.Elbv2;

public class LoadBalancersHandler : PathHandler
{
    private readonly IElbv2Api _elbv2;
    private readonly IEc2Api _ec2;

    public static Item CreateItem(string parentPath)
    {
        return new GenericContainerItem(parentPath, "load-balancers",
            "List and filter the load balancers within the current account and region");
    }
    
    public LoadBalancersHandler(string path, IPathHandlerContext context, IElbv2Api elbv2, IEc2Api ec2) : base(path, context)
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
        var loadBalancers = GetWithPaging(nextToken =>
            {
                var response = _elbv2.DescribeLoadBalancers(nextToken);

                return new PaginatedResponse<PSObject>
                {
                    PageOfResults = response.LoadBalancers,
                    NextToken = response.NextToken
                };
            }).ToArray();
        
        var securityGroupIds = loadBalancers
                .SelectMany(lb => lb.Property<IEnumerable<string>>("SecurityGroups") ?? Enumerable.Empty<string>())
                .Distinct()
                .ToList();

        var securityGroups = GetWithPaging(nextToken =>
        {
            var response = _ec2.DescribeSecurityGroups(new DescribeSecurityGroupsRequest
            {
                Ids = securityGroupIds,
                NextToken = nextToken
            });

            return new PaginatedResponse<PSObject>
            {
                PageOfResults = response.SecurityGroups.ToArray(),
                NextToken = response.NextToken
            };
        }).ToDictionary(sg => sg.Property<string>("GroupId")!);

        return loadBalancers.Select(lb => new LoadBalancerItem(Path, lb, securityGroups.MultiGet(lb.Property<IEnumerable<string>>("SecurityGroups")!)))
            .OrderBy(lb => lb.ItemName);
    }
}