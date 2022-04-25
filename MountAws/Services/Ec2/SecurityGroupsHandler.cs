using Amazon.EC2;
using Amazon.EC2.Model;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Ec2;

public class SecurityGroupsHandler : PathHandler
{
    private readonly IAmazonEC2 _ec2;

    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "security-groups",
            "Navigate the security groups in the current account and region");
    }
    
    public SecurityGroupsHandler(ItemPath path, IPathHandlerContext context, IAmazonEC2 ec2) : base(path, context)
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
        return _ec2.DescribeSecurityGroups(request)
            .Select(s => new SecurityGroupItem(Path, s))
            .OrderBy(s => s.GroupName);
    }
}