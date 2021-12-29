using System.Management.Automation;
using Amazon.EC2;
using Amazon.EC2.Model;
using MountAnything;
using MountAws.Api.Ec2;
using MountAws.Services.Core;

using static MountAws.PagingHelper;

namespace MountAws.Services.Ec2;

public class SecurityGroupsHandler : PathHandler
{
    private readonly IAmazonEC2 _ec2;

    public static IItem CreateItem(string parentPath)
    {
        return new GenericContainerItem(parentPath, "security-groups",
            "Navigate the security groups in the current account and region");
    }
    
    public SecurityGroupsHandler(string path, IPathHandlerContext context, IAmazonEC2 ec2) : base(path, context)
    {
        _ec2 = ec2;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return GetChildSecurityGroups(new DescribeSecurityGroupsRequest());
    }

    public override IEnumerable<IItem> GetChildItems(string filter)
    {
        var request = Ec2ApiExtensions.ParseSecurityGroupFilter(filter);
        return GetChildSecurityGroups(request);
    }

    private IEnumerable<IItem> GetChildSecurityGroups(DescribeSecurityGroupsRequest request)
    {
        return GetWithPaging(nextToken =>
        {
            request.NextToken = nextToken;
            var response = _ec2.DescribeSecurityGroups(request);

            return new PaginatedResponse<SecurityGroup>
            {
                PageOfResults = response.SecurityGroups.ToArray(),
                NextToken = response.NextToken
            };
        }).Select(s => new SecurityGroupItem(Path, s))
            .OrderBy(s => s.GroupName);
    }
}