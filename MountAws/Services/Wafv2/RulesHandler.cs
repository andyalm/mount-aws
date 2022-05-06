using Amazon.WAFV2;
using Amazon.WAFV2.Model;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Wafv2;

public class RulesHandler : PathHandler
{
    private readonly Lazy<WebACL> _webAcl;
    private readonly IAmazonWAFV2 _wafv2;

    public static IItem CreateItem(ItemPath parentPath, WebACL acl)
    {
        return new GenericContainerItem(parentPath, "rules",
            $"Navigate the {acl.Rules.Count} rules associated with the web acl");
    }
    
    public RulesHandler(ItemPath path, IPathHandlerContext context, Lazy<WebACL> webAcl, IAmazonWAFV2 wafv2) : base(path, context)
    {
        _webAcl = webAcl;
        _wafv2 = wafv2;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath, _webAcl.Value);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return _webAcl.Value.Rules.Select(r => new RuleItem(Path, r, _wafv2));
    }
}