using MountAnything;
using MountAws.Api.Ec2;

namespace MountAws.Services.Ec2;

public class VpcHandler : PathHandler
{
    private readonly IEc2Api _ec2;

    public VpcHandler(string path, IPathHandlerContext context, IEc2Api ec2) : base(path, context)
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
        yield break;
    }
}