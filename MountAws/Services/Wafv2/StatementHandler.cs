using Amazon.WAFV2;
using MountAnything;
using MountAws.Services.Wafv2.StatementNavigation;

namespace MountAws.Services.Wafv2;

public class StatementHandler : PathHandler
{
    private readonly IItemAncestor<RuleItem> _rule;
    private readonly IAmazonWAFV2 _wafv2;
    private readonly StatementPath _statementPath;

    public StatementHandler(ItemPath path, IPathHandlerContext context,
        IItemAncestor<RuleItem> rule, IAmazonWAFV2 wafv2, StatementPath statementPath) : base(path, context)
    {
        _rule = rule;
        _wafv2 = wafv2;
        _statementPath = statementPath;
    }

    protected override IItem? GetItemImpl()
    {
        var statement = GetStatement();

        if (statement == null)
        {
            return null;
        }
        
        return new StatementItem(ParentPath, statement, ItemName);
    }

    private IStatementNavigator? GetStatement()
    {
        return _rule.Item.UnderlyingObject.Statement.ToNavigator(_wafv2).StatementAt(_statementPath.Value);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        var statement = GetStatement();
        if (statement == null)
        {
            return Enumerable.Empty<IItem>();
        }

        return statement.GetChildren().Select(s => new StatementItem(Path, s, s.UniqueName()));
    }
}