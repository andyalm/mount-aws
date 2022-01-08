namespace MountAws.Services.Wafv2.StatementNavigation;

public class DefaultStatementNavigator : StatementNavigator<object>
{
    public DefaultStatementNavigator(object statement, int position) : base(statement, position)
    {
    }
}