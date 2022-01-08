using Amazon.WAFV2;
using Amazon.WAFV2.Model;
using MountAnything;

namespace MountAws.Services.Wafv2;

public class RuleHandler : PathHandler
{
    private readonly Lazy<WebACL> _webAcl;
    private readonly IAmazonWAFV2 _wafv2;

    public RuleHandler(ItemPath path, IPathHandlerContext context, Lazy<WebACL> webAcl, IAmazonWAFV2 wafv2) : base(path, context)
    {
        _webAcl = webAcl;
        _wafv2 = wafv2;
    }

    protected override IItem? GetItemImpl()
    {
        var rule = _webAcl.Value.Rules
            .SingleOrDefault(r => r.Name.Equals(ItemName, StringComparison.OrdinalIgnoreCase));

        return rule != null ? new RuleItem(ParentPath, rule, _wafv2) : null;
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

            yield return new StatementItem(Path, item.UnderlyingObject.Statement, _wafv2);
        }
    }
}