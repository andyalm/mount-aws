using MountAnything;
using MountAws.Api.Ec2;
using MountAws.Services.Core;

namespace MountAws.Services.Ec2;

public class VpcsHandler : PathHandler
{
    private readonly IEc2Api _ec2;

    public static IItem CreateItem(string parentPath)
    {
        return new GenericContainerItem(parentPath, "vpcs",
            "Navigate the vpcs in the current account and region");
    }

    public VpcsHandler(string path, IPathHandlerContext context, IEc2Api ec2) : base(path, context)
    {
        _ec2 = ec2;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return _ec2.DescribeVpcs()
            .Select(v => new VpcItem(Path, v));
    }
}