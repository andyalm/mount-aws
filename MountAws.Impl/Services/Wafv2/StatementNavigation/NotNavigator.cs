using Amazon.WAFV2;
using Amazon.WAFV2.Model;

namespace MountAws.Services.Wafv2.StatementNavigation;

public class NotNavigator : StatementNavigator<NotStatement>
{
    public NotNavigator(NotStatement not, int position, IAmazonWAFV2 wafv2) : base(not, position)
    {
        NegatedStatement = not.Statement.ToNavigator(wafv2);
        Description = $"not {NegatedStatement.Description}";
    }

    public IStatementNavigator NegatedStatement { get; }
    
    public override string Description { get; }

    public override IEnumerable<IStatementNavigator> GetChildren()
    {
        yield return NegatedStatement;
    }
}