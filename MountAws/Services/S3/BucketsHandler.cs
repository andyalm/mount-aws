using Amazon.S3;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.S3;

public class BucketsHandler : PathHandler
{
    private readonly IAmazonS3 _s3;

    public static Item CreateItem(string parentPath)
    {
        return new GenericContainerItem(parentPath, "buckets",
            "The s3 buckets in the current account/region");
    }
    
    public BucketsHandler(string path, IPathHandlerContext context, IAmazonS3 s3) : base(path, context)
    {
        _s3 = s3;
    }

    protected override Item? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<Item> GetChildItemsImpl()
    {
        return _s3.ListBucketsAsync()
            .GetAwaiter()
            .GetResult()
            .Buckets
            .Select(b => new BucketItem(Path, b.BucketName));
    }
}