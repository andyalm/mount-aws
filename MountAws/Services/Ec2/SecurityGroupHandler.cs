using Amazon.EC2;
using MountAnything;

namespace MountAws.Services.Ec2;

public class SecurityGroupHandler : PathHandler
{
    private readonly IAmazonEC2 _ec2;

    public SecurityGroupHandler(ItemPath path, IPathHandlerContext context, IAmazonEC2 ec2) : base(path, context)
    {
        _ec2 = ec2;
    }

    protected override IItem? GetItemImpl()
    {
        var request = Ec2ApiExtensions.ParseSecurityGroupFilter(ItemName);
        var securityGroups = _ec2.DescribeSecurityGroups(request).ToArray();
        WriteDebug($"Found {securityGroups.Length} security groups");
        if (securityGroups.Length == 1)
        {
            return new SecurityGroupItem(ParentPath, securityGroups.Single());
        }

        return null;
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield break;
    }
}