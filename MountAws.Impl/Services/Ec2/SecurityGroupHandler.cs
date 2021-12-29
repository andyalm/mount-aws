using MountAnything;
using MountAws.Api.Ec2;

namespace MountAws.Services.Ec2;

public class SecurityGroupHandler : PathHandler
{
    private readonly IEc2Api _ec2;

    public SecurityGroupHandler(string path, IPathHandlerContext context, IEc2Api ec2) : base(path, context)
    {
        _ec2 = ec2;
    }

    protected override IItem? GetItemImpl()
    {
        var request = Ec2ApiExtensions.ParseSecurityGroupFilter(ItemName);
        var securityGroups = _ec2.DescribeSecurityGroups(request).SecurityGroups.ToArray();
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