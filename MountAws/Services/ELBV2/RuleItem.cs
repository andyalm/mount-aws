using System.Management.Automation;
using Amazon.ElasticLoadBalancingV2.Model;

namespace MountAws.Services.ELBV2;

public class RuleItem : AwsItem
{
    private readonly string _parentPath;
    public Rule Rule { get; }

    public RuleItem(string parentPath, Rule rule)
    {
        _parentPath = parentPath;
        Rule = rule;
    }

    public override string FullPath => AwsPath.Combine(_parentPath, ItemName);
    public override string ItemName => Rule.Priority;
    public override object UnderlyingObject => Rule;
    public override string ItemType => "Rule";
    public override bool IsContainer => true;

    public string ConditionDescription => string.Join("|", Rule.Conditions.Select(c => c.Description()));
    public string ActionDescription => ActionItem.Create(FullPath, Rule.Actions.Last()).Description;

    public override void CustomizePSObject(PSObject psObject)
    {
        base.CustomizePSObject(psObject);
        psObject.Properties.Add(new PSNoteProperty(nameof(ActionDescription), ActionDescription));
        psObject.Properties.Add(new PSNoteProperty(nameof(ConditionDescription), ConditionDescription));
    }
}