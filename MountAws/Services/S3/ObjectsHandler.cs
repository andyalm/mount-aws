using Amazon.S3;
using Amazon.S3.Model;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.S3;

public class ObjectsHandler : PathHandler
{
    public static Item CreateItem(string parentPath)
    {
        return new GenericContainerItem(parentPath, "objects",
            "Navigate the objects in the s3 bucket like a filesystem");
    }

    private readonly IAmazonS3 _s3;
    private readonly CurrentBucket _currentBucket;

    public ObjectsHandler(string path, IPathHandlerContext context, IAmazonS3 s3, CurrentBucket currentBucket) : base(path, context)
    {
        _s3 = s3;
        _currentBucket = currentBucket;
    }

    protected override Item? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<Item> GetChildItemsImpl()
    {
        return _s3.ListChildItems(_currentBucket.Name, Path);
    }
}