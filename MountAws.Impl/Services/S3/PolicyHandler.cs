using System.Management.Automation.Provider;
using Amazon.S3;
using MountAnything;
using MountAnything.Content;

namespace MountAws.Services.S3;

public class PolicyHandler : PathHandler, IContentReaderHandler
{
    private readonly IAmazonS3 _s3;
    private string BucketName { get; }

    public PolicyHandler(string path, IPathHandlerContext context, IAmazonS3 s3) : base(path, context)
    {
        _s3 = s3;
        BucketName = ItemPath.GetLeaf(ParentPath);
    }

    protected override IItem? GetItemImpl()
    {
        return new PolicyItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield break;
    }

    public IContentReader GetContentReader()
    {
        var policy = GetRawPolicy();

        return new StringContentReader(policy);
    }

    private string GetRawPolicy()
    {
        return _s3.GetBucketPolicy(BucketName);
    }
}