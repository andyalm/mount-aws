using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.S3;

public class S3RootHandler : PathHandler
{
    public static Item CreateItem(string parentPath)
    {
        return new GenericContainerItem(parentPath, "s3",
            "Navigate s3 buckets as a filesystem");
    }
    
    public S3RootHandler(string path, IPathHandlerContext context) : base(path, context)
    {
    }

    protected override Item? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<Item> GetChildItemsImpl()
    {
        yield return BucketsHandler.CreateItem(Path);
    }
}