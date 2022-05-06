using Amazon.EC2;
using MountAnything;

namespace MountAws.Services.Ec2;

public class ImageHandler : PathHandler
{
    private readonly IAmazonEC2 _ec2;

    public ImageHandler(ItemPath path, IPathHandlerContext context, IAmazonEC2 ec2) : base(path, context)
    {
        _ec2 = ec2;
    }
    
    private string ImageId => Cache.ResolveAlias<ImageItem>(ItemName, ImageIdFromName);

    protected override IItem? GetItemImpl()
    {
        try
        {
            var image = _ec2.DescribeImage(ImageId);

            return new ImageItem(ParentPath, image);
        }
        catch (ImageNotFoundException)
        {
            return null;
        }
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield break;
    }
    
    private string ImageIdFromName(string name)
    {
        return _ec2.DescribeImage(name).ImageId;
    }
}