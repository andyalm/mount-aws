using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Cloudwatch;

public class LogGroupsHandler : PathHandler
{
    private readonly LogGroupNavigator _navigator;

    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "log-groups",
            "Navigate cloudwatch logs");
    }
    
    public LogGroupsHandler(ItemPath path, IPathHandlerContext context, LogGroupNavigator navigator) : base(path, context)
    {
        _navigator = navigator;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        var discoveredChildItems = _navigator.ListChildItems(Path);
        bool foundAws = false;
        foreach (var logGroupItem in discoveredChildItems)
        {
            yield return logGroupItem;
            if (!foundAws)
            {
                foundAws = logGroupItem.ItemName.Equals("aws");
            }
        }
        // the cloudwatch api won't return everything, unfortunately. But we know there is an aws folder.
        if (!foundAws)
        {
            yield return new LogGroupItem(Path, new ItemPath("aws"));
        }
    }
}