using Amazon.WAFV2;
using Amazon.WAFV2.Model;
using MountAnything;

namespace MountAws.Services.Wafv2;

public class RuleHandler : PathHandler
{
    private readonly Lazy<WebACL> _webAcl;
    
    public RuleHandler(ItemPath path, IPathHandlerContext context, Lazy<WebACL> webAcl) : base(path, context)
    {
        _webAcl = webAcl;
    }

    protected override IItem? GetItemImpl()
    {
        var rule = _webAcl.Value.Rules
            .SingleOrDefault(r => r.Name.Equals(ItemName, StringComparison.OrdinalIgnoreCase));

        return rule != null ? new RuleItem(ParentPath, rule) : null;
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        if (GetItem() is RuleItem item)
        {
            var actionItem = item.UnderlyingObject.ActionItem(Path);
            if (actionItem != null)
            {
                yield return actionItem;
            }

            var overrideActionItem = item.UnderlyingObject.OverrideActionItem(Path);
            if (overrideActionItem != null)
            {
                yield return overrideActionItem;
            }
        }
    }
}