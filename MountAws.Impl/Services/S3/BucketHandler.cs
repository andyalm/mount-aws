using MountAnything;
using MountAws.Api.S3;

namespace MountAws.Services.S3;

public class BucketHandler : PathHandler
{
    private readonly IS3Api _s3;

    public BucketHandler(string path, IPathHandlerContext context, IS3Api s3) : base(path, context)
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