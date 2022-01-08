namespace MountAws.Services.Wafv2.StatementNavigation;

public interface IStatementNavigator
{
    string Name { get; }
    
    string Description { get; }

    IEnumerable<IStatementNavigator> GetChildren();
}