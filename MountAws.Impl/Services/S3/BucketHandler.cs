using Amazon.S3;
using MountAnything;

namespace MountAws.Services.S3;

public class BucketHandler : PathHandler
{
    private readonly IAmazonS3 _s3;

    public BucketHandler(ItemPath path, IPathHandlerContext context, IAmazonS3 s3) : base(path, context)
    {
        _s3 = s3;
    }

    protected override IItem? GetItemImpl()
    {
        var exists = _s3.BucketExists(ItemName);
        if (exists)
        {
            return new BucketItem(ParentPath, ItemName);
        }

        return null;
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield return new PolicyItem(Path);
        yield return ObjectsHandler.CreateItem(Path);
    }
}