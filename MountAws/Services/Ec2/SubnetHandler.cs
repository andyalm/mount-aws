using Amazon.EC2;
using MountAnything;
using MountAws.Api.Ec2;

namespace MountAws.Services.Ec2;

public class SubnetHandler : PathHandler
{
    private readonly IAmazonEC2 _ec2;

    public SubnetHandler(ItemPath path, IPathHandlerContext context, IAmazonEC2 ec2) : base(path, context)
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