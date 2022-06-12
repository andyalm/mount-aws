using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Cloudwatch;

public class RootHandler : PathHandler
{
    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "cloudwatch",
            "Navigate cloudwatch things as a virtual hierarchy");
    }
    
    public RootHandler(ItemPath path, IPathHandlerContext context) : base(path, context)
    {
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield return LogGroupsHandler.CreateItem(Path);
        yield return MetricsHandler.CreateItem(Path);
    }
}