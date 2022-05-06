using Amazon.WAFV2.Model;
using MountAnything;

namespace MountAws.Services.Wafv2;

public abstract class ActionPathHandler : PathHandler
{
    public ActionPathHandler(ItemPath path, IPathHandlerContext context) : base(path, context)
    {
    }

    protected abstract ActionItem? GetActionItem();
    
    protected override IItem? GetItemImpl()
    {
        return GetActionItem();
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

    private IEnumerable<IItem> GetAllowChildItems(AllowAction allow)
    {
        var customHeadersItem = allow.GetCustomHeadersChildItem(Path);
        if (customHeadersItem != null)
        {
            yield return customHeadersItem;
        }
    }

    private IEnumerable<IItem> GetBlockChildItems(BlockAction block)
    {
        var customHeadersItem = block.GetCustomHeadersChildItem(Path);
        if (customHeadersItem != null)
        {
            yield return customHeadersItem;
        }
    }
}

public class DefaultActionHandler : ActionPathHandler
{
    private readonly Lazy<WebACL> _webAcl;

    public DefaultActionHandler(ItemPath path, IPathHandlerContext context, Lazy<WebACL> webAcl) : base(path, context)
    {
        _webAcl = webAcl;
    }

    protected override ActionItem? GetActionItem()
    {
        return _webAcl.Value.DefaultActionItem(ParentPath);
    }
}

public class RuleActionHandler : ActionPathHandler
{
    private readonly IItemAncestor<RuleItem> _ruleItem;

    public RuleActionHandler(ItemPath path, IPathHandlerContext context, IItemAncestor<RuleItem> ruleItem) : base(path, context)
    {
        _ruleItem = ruleItem;
    }

    protected override ActionItem? GetActionItem()
    {
        return _ruleItem.Item.UnderlyingObject.ActionItem(ParentPath);
    }
}