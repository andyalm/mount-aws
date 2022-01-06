using Amazon.WAFV2;
using Amazon.WAFV2.Model;
using MountAnything;

namespace MountAws.Services.Wafv2;

public class DefaultActionHandler : PathHandler
{
    private readonly WebAclHandler _parentHandler;
    
    public DefaultActionHandler(ItemPath path, IPathHandlerContext context, IAmazonWAFV2 wafv2, Scope scope) : base(path, context)
    {
        _parentHandler = new WebAclHandler(ParentPath, context, wafv2, scope);
    }

    protected override IItem? GetItemImpl()
    {
        return _parentHandler.GetDefaultActionItem();
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        var actionItem = GetItem() as ActionItem;
        return actionItem?.ActionObject switch
        {
            AllowAction allow => GetAllowChildItems(allow),
            BlockAction block => GetBlockChildItems(block),
            _ => Enumerable.Empty<IItem>()
        };
    }

    public CustomHeadersItem? GetCustomHeadersChildItem()
    {
        var actionItem = GetItem() as ActionItem;
        return actionItem?.ActionObject switch
        {
            AllowAction allow => GetCustomHeadersChildItem(allow),
            BlockAction block => GetCustomHeadersChildItem(block),
            _ => null
        };
    }

    public CustomHeadersItem? GetCustomHeadersChildItem(AllowAction allow)
    {
        return allow.CustomRequestHandling?.InsertHeaders.Any() == true
            ? CustomHeadersHandler.CreateItem(Path,
                "custom-request-headers",
                "Lists the custom request headers that will be inserted by this action",
                allow.CustomRequestHandling.InsertHeaders)
            : null;
    }
    
    public CustomHeadersItem? GetCustomHeadersChildItem(BlockAction block)
    {
        return block.CustomResponse?.ResponseHeaders.Any() == true
            ? CustomHeadersHandler.CreateItem(Path,
                "custom-response-headers",
                "Lists the custom response headers that will be inserted by this action",
                block.CustomResponse.ResponseHeaders)
            : null;
    }
    
    private IEnumerable<IItem> GetAllowChildItems(AllowAction allow)
    {
        var customHeadersItem = GetCustomHeadersChildItem(allow);
        if (customHeadersItem != null)
        {
            yield return customHeadersItem;
        }
    }

    private IEnumerable<IItem> GetBlockChildItems(BlockAction block)
    {
        var customHeadersItem = GetCustomHeadersChildItem(block);
        if (customHeadersItem != null)
        {
            yield return customHeadersItem;
        }
    }
}