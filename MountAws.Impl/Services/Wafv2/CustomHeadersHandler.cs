using Amazon.WAFV2;
using Amazon.WAFV2.Model;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Wafv2;

public class CustomHeadersHandler : PathHandler
{
    public static CustomHeadersItem CreateItem(ItemPath parentPath, string itemName, string description, IEnumerable<CustomHTTPHeader> customHeaders)
    {
        return new CustomHeadersItem(parentPath, itemName, description, customHeaders);
    }

    private readonly DefaultActionHandler _parentHandler;
    
    public CustomHeadersHandler(ItemPath path, IPathHandlerContext context, IAmazonWAFV2 wafv2, Scope scope) : base(path, context)
    {
        _parentHandler = new DefaultActionHandler(ParentPath, context, wafv2, scope);
    }

    protected override IItem? GetItemImpl()
    {
        return _parentHandler.GetCustomHeadersChildItem();
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