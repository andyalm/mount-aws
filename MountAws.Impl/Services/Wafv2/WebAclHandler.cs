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
        var webAcl = _wafv2.ListWebAcls(_scope)
            .SingleOrDefault(i => i.Name.Equals(ItemName, StringComparison.OrdinalIgnoreCase));

        return webAcl != null ? new WebAclItem(ParentPath, webAcl) : null;
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
        yield return RulesHandler.CreateItem(Path, acl);
    }

    public override IEnumerable<IItemProperty> GetItemProperties(HashSet<string> propertyNames, Func<ItemPath, string> pathResolver)
    {
        return GetWebAcl()?.ToPSObject().AsItemProperties() ?? Enumerable.Empty<IItemProperty>();
    }
    

    private WebACL? GetWebAcl()
    {
        if (GetItem() is not WebAclItem item)
        {
            return null;
        }

        return _wafv2.GetWebAcl(_scope, (item.Id, item.ItemName));
    }
}