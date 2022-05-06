using Amazon.WAFV2;
using Amazon.WAFV2.Model;

namespace MountAws.Services.Wafv2.StatementNavigation;

public class AndNavigator : StatementNavigator<AndStatement>
{
    private readonly IAmazonWAFV2 _wafv2;

    public AndNavigator(AndStatement statement, int position, IAmazonWAFV2 wafv2) : base(statement, position)
    {
        _wafv2 = wafv2;
        Description = string.Join(" and ", GetChildren().Select(c => c.Description));
    }

    public override string Description { get; }
    public override IEnumerable<IStatementNavigator> GetChildren()
    {
        return Statement.Statements.Select((s, i) => s.ToNavigator(_wafv2, i + 1));
    }
}