using System.Net;
using Amazon.S3;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.S3;

public class BucketHandler : PathHandler
{
    private readonly IAmazonS3 _s3;

    public BucketHandler(string path, IPathHandlerContext context, IAmazonS3 s3) : base(path, context)
    {
        _s3 = s3;
    }

    protected override Item? GetItemImpl()
    {
        var response = _s3.GetBucketPolicyAsync(ItemName).GetAwaiter().GetResult();
        if (response.HttpStatusCode == HttpStatusCode.OK)
        {
            return new BucketItem(ParentPath, ItemName);
        }

        return null;
    }

    protected override IEnumerable<Item> GetChildItemsImpl()
    {
        yield return new PolicyItem(Path);
        yield return ObjectsHandler.CreateItem(Path);
    }
}