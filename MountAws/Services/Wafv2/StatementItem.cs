using System.Management.Automation;
using Amazon.WAFV2;
using Amazon.WAFV2.Model;
using MountAnything;
using MountAws.Services.Core;
using MountAws.Services.Wafv2.StatementNavigation;

namespace MountAws.Services.Wafv2;

public class StatementItem : AwsItem
{
    public StatementItem(ItemPath parentPath, Statement underlyingObject, IAmazonWAFV2 wafv2, string? itemName = null) : base(parentPath, new PSObject(underlyingObject))
    {
        Navigator = underlyingObject.ToNavigator(wafv2);
        ItemName = itemName ?? Navigator.Name;
    }
    
    public StatementItem(ItemPath parentPath, IStatementNavigator statement, string? itemName = null) : base(parentPath, new PSObject(statement.UnderlyingObject))
    {
        Navigator = statement;
        ItemName = itemName ?? Navigator.Name;
    }

    public IStatementNavigator Navigator { get; }
    public override string ItemName { get; }
    public string Description => Navigator.Description;
    
    protected override string TypeName => typeof(GenericContainerItem).FullName!;

    public override bool IsContainer => true;

    protected override void CustomizePSObject(PSObject psObject)
    {
        base.CustomizePSObject(psObject);
        psObject.Properties.Add(new PSNoteProperty(nameof(Description), Description));
    }
}