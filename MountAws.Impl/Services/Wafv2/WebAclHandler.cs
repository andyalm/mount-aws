using Amazon.WAFV2;
using Amazon.WAFV2.Model;
using MountAnything;

namespace MountAws.Services.Wafv2;

public class WebAclHandler : PathHandler
{
    private readonly IAmazonWAFV2 _wafv2;
    private readonly Scope _scope;

    public WebAclHandler(ItemPath path, IPathHandlerContext context, IAmazonWAFV2 wafv2, Scope scope) : base(path, context)
    {
        _wafv2 = wafv2;
        _scope = scope;
    }

    protected override IItem? GetItemImpl()
    {
        var parentHandler = new WebAclsHandler(ParentPath, Context, _wafv2, _scope);
        return parentHandler.GetChildItems(Freshness.Default)
            .Cast<WebAclItem>()
            .SingleOrDefault(i => i.ItemName.Equals(ItemName, StringComparison.OrdinalIgnoreCase));
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        var acl = GetWebAcl();
        if (acl == null)
        {
            yield break;
        }

        var defaultAction = acl.DefaultActionItem(Path);
        if (defaultAction != null)
        {
            yield return defaultAction;
        }
    }

    public override IEnumerable<IItemProperty> GetItemProperties(HashSet<string> propertyNames, Func<ItemPath, string> pathResolver)
    {
        return GetWebAcl()?.ToPSObject().AsItemProperties() ?? Enumerable.Empty<IItemProperty>();
    }

    public ActionItem? GetDefaultActionItem()
    {
        return GetWebAcl()?.DefaultActionItem(Path);
    }

    public WebACL? GetWebAcl()
    {
        if (GetItem() is not WebAclItem item)
        {
            return null;
        }

        return _wafv2.GetWebAcl(_scope, (item.Id, item.ItemName));
    }
}