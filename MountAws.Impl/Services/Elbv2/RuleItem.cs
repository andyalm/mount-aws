using System.Management.Automation;
using Amazon.ElasticLoadBalancingV2.Model;
using MountAnything;
using Action = Amazon.ElasticLoadBalancingV2.Model.Action;

namespace MountAws.Services.Elbv2;

public class RuleItem : AwsItem<Rule>
{
    public string RuleArn { get; }
    public IEnumerable<Action> Actions => UnderlyingObject.Actions;
    public IEnumerable<RuleCondition> Conditions => UnderlyingObject.Conditions;
    public string ConditionDescription { get; }
    public RuleItem(ItemPath parentPath, Rule rule) : base(parentPath, rule)
    {
        RuleArn = rule.RuleArn;
        ConditionDescription = string.Join("|", Conditions.Select(ToConditionDescription));
    }

    public override string ItemName => UnderlyingObject.Priority;
    public override string ItemType => Elbv2ItemTypes.Rule;
    public override bool IsContainer => true;
    public string ActionDescription => ActionItem.Create(FullPath, Actions.Last()).Description;

    protected override void CustomizePSObject(PSObject psObject)
    {
        base.CustomizePSObject(psObject);
        psObject.Properties.Add(new PSNoteProperty(nameof(ActionDescription), ActionDescription));
        psObject.Properties.Add(new PSNoteProperty(nameof(ConditionDescription), ConditionDescription));
    }
    
    private static string ToConditionDescription(RuleCondition condition)
    {
        var field = condition.Field;
        var values = condition.Values;
        switch (field)
        {
            case "http-header":
                var httpHeaderConfig = condition.HttpHeaderConfig;
                return
                    $"{field} {httpHeaderConfig.HttpHeaderName} matches ({string.Join(",", values)})";
            default:
                return $"{field} matches ({string.Join(",", values)})";
        }
    }
}