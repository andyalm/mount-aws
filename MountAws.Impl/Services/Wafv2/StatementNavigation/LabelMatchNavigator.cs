using Amazon.WAFV2.Model;

namespace MountAws.Services.Wafv2.StatementNavigation;

public class LabelMatchNavigator : StatementNavigator<LabelMatchStatement>
{
    public LabelMatchNavigator(LabelMatchStatement statement) : base(statement)
    {
        Description = $"{statement.Scope.Value.ToLower()} matches {statement.Key}";
    }

    public override string Description { get; }
}