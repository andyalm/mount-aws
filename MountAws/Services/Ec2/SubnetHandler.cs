using MountAnything;
using MountAws.Api.Ec2;

namespace MountAws.Services.Ec2;

public class SubnetHandler : PathHandler
{
    private readonly IEc2Api _ec2;

    public SubnetHandler(string path, IPathHandlerContext context, IEc2Api ec2) : base(path, context)
    {
        _ec2 = ec2;
    }

    protected override IItem? GetItemImpl()
    {
        try
        {
            return new SubnetItem(ParentPath, _ec2.DescribeSubnet(ItemName));
        }
        catch (SubnetNotFoundException)
        {
            return null;
        }
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield break;
    }
}