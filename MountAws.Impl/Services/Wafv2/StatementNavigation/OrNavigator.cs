using Amazon.WAFV2;
using Amazon.WAFV2.Model;

namespace MountAws.Services.Wafv2.StatementNavigation;

public class OrNavigator : StatementNavigator<OrStatement>
{
    private readonly IAmazonWAFV2 _wafv2;

    public OrNavigator(OrStatement statement, IAmazonWAFV2 wafv2) : base(statement)
    {
        _wafv2 = wafv2;
        Description = string.Join(" and ", GetChildren().Select(c => c.Description));
    }

    public override string Description { get; }
    public override IEnumerable<IStatementNavigator> GetChildren()
    {
        return Statement.Statements.Select(s => s.ToNavigator(_wafv2));
    }
}