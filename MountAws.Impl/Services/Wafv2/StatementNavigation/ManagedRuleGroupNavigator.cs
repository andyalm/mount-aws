using Amazon.WAFV2.Model;

namespace MountAws.Services.Wafv2.StatementNavigation;

public class ManagedRuleGroupNavigator : StatementNavigator<ManagedRuleGroupStatement>
{
    public ManagedRuleGroupNavigator(ManagedRuleGroupStatement statement) : base(statement)
    {
        Name = $"{statement.VendorName}:{statement.Name}";
        Description = $"ManagedRule: {Name}";
    }

    public override string Name { get; }
    public override string Description { get; }
}