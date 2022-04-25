using Amazon.EC2;
using MountAnything;
using MountAws.Api.Ec2;

namespace MountAws.Services.Ec2;

public class VpcHandler : PathHandler
{
    private readonly IAmazonEC2 _ec2;

    public VpcHandler(ItemPath path, IPathHandlerContext context, IAmazonEC2 ec2) : base(path, context)
    {
        _ec2 = ec2;
    }

    protected override IItem? GetItemImpl()
    {
        try
        {
            var vpc = _ec2.DescribeVpc(ItemName);
            return new VpcItem(ParentPath, vpc);
        }
        catch (VpcNotFoundException)
        {
            return null;
        }
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        var vpc = GetItem(Freshness.Fastest);
        if (vpc != null)
        {
            return _ec2.DescribeSubnetsByVpc(vpc.ItemName)
                .Select(s => new SubnetItem(Path, s));
        }

        return Enumerable.Empty<IItem>();
    }
}