using Amazon.WAFV2.Model;
using MountAnything;

namespace MountAws.Services.Wafv2;

public class CustomHeadersHandler : PathHandler
{
    private readonly ActionItem _defaultAction;

    public static CustomHeadersItem CreateItem(ItemPath parentPath, string itemName, string description, IEnumerable<CustomHTTPHeader> customHeaders)
    {
        return new CustomHeadersItem(parentPath, itemName, description, customHeaders);
    }

    public CustomHeadersHandler(ItemPath path, IPathHandlerContext context, ActionItem defaultAction) : base(path, context)
    {
        _defaultAction = defaultAction;
    }

    protected override IItem? GetItemImpl()
    {
        return _defaultAction.GetCustomHeadersChildItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        if (GetItem() is CustomHeadersItem item)
        {
            return item.CustomHeaders.Select(h => new CustomHeaderItem(Path, h));
        }
        
        return Enumerable.Empty<IItem>();
    }
}