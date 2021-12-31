using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.S3;

public class S3RootHandler : PathHandler
{
    public static Item CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "s3",
            "Navigate s3 buckets as a filesystem");
    }
    
    public S3RootHandler(ItemPath path, IPathHandlerContext context) : base(path, context)
    {
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield return BucketsHandler.CreateItem(Path);
    }
}