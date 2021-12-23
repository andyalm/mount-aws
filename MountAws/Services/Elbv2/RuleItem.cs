using System.Management.Automation;
using MountAnything;
using MountAws.Api;

namespace MountAws.Services.Elbv2;

public class RuleItem : Item
{
    public IEnumerable<PSObject> Actions { get; }
    public IEnumerable<PSObject> Conditions { get; }
    public string ConditionDescription { get; }
    public RuleItem(string parentPath, PSObject rule) : base(parentPath, rule)
    {
        Actions = Property<IEnumerable<PSObject>>("Actions")!;
        Conditions = Property<IEnumerable<PSObject>>("Conditions")!;
        ConditionDescription = string.Join("|", Conditions.Select(ToConditionDescription));
    }

    public override string ItemName => Property<string>("Priority")!;
    public override string ItemType => Elbv2ItemTypes.Rule;
    public override bool IsContainer => true;
    public string ActionDescription => ActionItem.Create(FullPath, Property<IEnumerable<PSObject>>("Actions")!.Last()).Description;

    public override void CustomizePSObject(PSObject psObject)
    {
        base.CustomizePSObject(psObject);
        psObject.Properties.Add(new PSNoteProperty(nameof(ActionDescription), ActionDescription));
        psObject.Properties.Add(new PSNoteProperty(nameof(ConditionDescription), ConditionDescription));
    }
    
    private static string ToConditionDescription(PSObject condition)
    {
        var field = condition.Property<string>("Field");
        var values = condition.Property<IEnumerable<PSObject>>("Values")!;
        switch (field)
        {
            case "http-header":
                var httpHeaderConfig = condition.Property<PSObject>("HttpHeaderConfig")!;
                return
                    $"{field} {httpHeaderConfig.Property<string>("HttpHeaderName")} matches ({string.Join(",", values)})";
            default:
                return $"{field} matches ({string.Join(",", values)})";
        }
    }
}