using Amazon.EC2;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Ec2;

public class ImagesHandler : PathHandler
{
    private readonly IAmazonEC2 _ec2;

    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "images",
            "Navigate AMI's available in the current account and region");
    }
    
    public ImagesHandler(ItemPath path, IPathHandlerContext context, IAmazonEC2 ec2) : base(path, context)
    {
        _ec2 = ec2;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return _ec2.DescribeImages().Select(i => new ImageItem(Path, i));
    }

    public override IEnumerable<IItem> GetChildItems(string filter)
    {
        return _ec2.QueryImages(filter).Select(i => new ImageItem(Path, i));
    }
}