namespace MountAws.Services.Wafv2.StatementNavigation;

public interface IStatementNavigator
{
    int Position { get; }
    string Name { get; }
    string Description { get; }
    object UnderlyingObject { get; }

    IEnumerable<IStatementNavigator> GetChildren();
}