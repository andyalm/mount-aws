using Amazon.WAFV2;
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
        var webAcl = _wafv2.GetWebAcl(_scope, ItemName);

        return new WebAclItem(ParentPath, webAcl);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield break;
    }
}