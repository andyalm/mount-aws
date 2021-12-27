using System.Management.Automation;
using MountAnything;
using MountAws.Api;

namespace MountAws.Services.Elbv2;

public class FixedActionItem : ActionItem
{
    public FixedActionItem(string parentPath, PSObject action) : base(parentPath, action) { }

    public override string ItemType => Elbv2ItemTypes.FixedAction;
    public override bool IsContainer => false;
    public override string Description => $"Fixed {Property<PSObject>("FixedResponseConfig")!.Property<string>("StatusCode")} response";
}