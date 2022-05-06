using Amazon.WAFV2.Model;

namespace MountAws.Services.Wafv2.StatementNavigation;

public class RuleGroupReferenceNavigator : StatementNavigator<RuleGroupReferenceStatement>
{
    public RuleGroupReferenceNavigator(RuleGroupReferenceStatement reference, int position) : base(reference, position)
    {
        Description = reference.ARN;
    }

    public override string Description { get; }
}