using System.Management.Automation.Provider;
using MountAnything;
using MountAnything.Content;
using MountAws.Api.S3;

namespace MountAws.Services.S3;

public class PolicyHandler : PathHandler, IContentReaderHandler
{
    private readonly IS3Api _s3;
    private string BucketName { get; }

    public PolicyHandler(string path, IPathHandlerContext context, IS3Api s3) : base(path, context)
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