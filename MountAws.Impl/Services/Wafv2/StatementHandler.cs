using System.Management.Automation;
using Amazon.WAFV2;
using Amazon.WAFV2.Model;
using MountAnything;
using MountAws.Services.Core;
using MountAws.Services.Wafv2.StatementNavigation;

namespace MountAws.Services.Wafv2;

public class StatementHandler : PathHandler
{
    private readonly IItemAncestor<RuleItem> _rule;
    private readonly IAmazonWAFV2 _wafv2;


    public StatementHandler(ItemPath path, IPathHandlerContext context, IItemAncestor<RuleItem> rule, IAmazonWAFV2 wafv2) : base(path, context)
    {
        _rule = rule;
        _wafv2 = wafv2;
    }

    protected override IItem? GetItemImpl()
    {
        return new StatementItem(ParentPath, _rule.Item.UnderlyingObject.Statement, _wafv2, "statement");
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield break;
    }
}

public class StatementItem : AwsItem<Statement>
{
    public StatementItem(ItemPath parentPath, Statement underlyingObject, IAmazonWAFV2 wafv2, string? itemName = null) : base(parentPath, underlyingObject)
    {
        Navigator = underlyingObject.ToNavigator(wafv2);
        ItemName = itemName ?? Navigator.Name;
    }

    public IStatementNavigator Navigator { get; }
    public override string ItemName { get; }
    public string Description => Navigator.Description;
    
    protected override string TypeName => typeof(GenericContainerItem).FullName!;

    public override bool IsContainer => false;

    protected override void CustomizePSObject(PSObject psObject)
    {
        base.CustomizePSObject(psObject);
        psObject.Properties.Add(new PSNoteProperty(nameof(Description), Description));
    }
}