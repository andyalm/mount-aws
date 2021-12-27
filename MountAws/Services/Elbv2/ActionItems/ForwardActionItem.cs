using System.Management.Automation;
using MountAnything;
using MountAws.Api;
using MountAws.Api.Elbv2;

namespace MountAws.Services.Elbv2;

public class ForwardActionItem : ActionItem
{
    public ForwardActionItem(string parentPath, PSObject action) : base(parentPath, action)
    {
        TargetGroupArn = string.IsNullOrEmpty(action.Property<string>("TargetGroupArn"))
            ? action.Property<PSObject>("ForwardConfig")!.Property<IEnumerable<PSObject>>("TargetGroups")!.Single().Property<string>("TargetGroupArn")!
            : action.Property<string>("TargetGroupArn")!;
        TargetGroupName = Elbv2ApiExtensions.TargetGroupName(TargetGroupArn);
    }
    public override string ItemType => Elbv2ItemTypes.ForwardAction;
    public override bool IsContainer => true;

    public override string Description => $"Forwards to {TargetGroupName}";
    public string TargetGroupArn { get; }
    public string TargetGroupName { get; }

    public override IEnumerable<Item> GetChildren(IElbv2Api elbv2)
    {
        return new[]
        {
            new TargetGroupItem(FullPath, elbv2.GetTargetGroup(TargetGroupArn))
        };
    }
}