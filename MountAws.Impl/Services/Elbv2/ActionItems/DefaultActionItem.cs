using System.Management.Automation;
using MountAnything;

namespace MountAws.Services.Elbv2;

public class DefaultActionItem : ActionItem
{
    public DefaultActionItem(string parentPath, PSObject action) : base(parentPath, action) {}

    public override string ItemType => Elbv2ItemTypes.Action;
    public override bool IsContainer => false;
    public override string Description => ItemType;
}