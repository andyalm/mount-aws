using System.Management.Automation;
using MountAnything;
using MountAws.Api.Elbv2;
using MountAws.Services.Core;

using static MountAws.PagingHelper;

namespace MountAws.Services.Elbv2;

public class LoadBalancersHandler : PathHandler
{
    private readonly IElbv2Api _elbv2;

    public static Item CreateItem(string parentPath)
    {
        return new GenericContainerItem(parentPath, "load-balancers",
            "List and filter the load balancers within the current account and region");
    }
    
    public LoadBalancersHandler(string path, IPathHandlerContext context, IElbv2Api elbv2) : base(path, context)
    {
        _elbv2 = elbv2;
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
        return GetWithPaging(nextToken =>
            {
                var response = _elbv2.DescribeLoadBalancers(nextToken);

                return new PaginatedResponse<PSObject>
                {
                    PageOfResults = response.LoadBalancers,
                    NextToken = response.NextToken
                };
            }).Select(lb => new LoadBalancerItem(Path, lb));
    }
}