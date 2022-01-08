namespace MountAws.Services.Wafv2.StatementNavigation;

public abstract class StatementNavigator<TStatement> : IStatementNavigator where TStatement : class
{
    protected TStatement Statement { get; }

    public StatementNavigator(TStatement statement)
    {
        Statement = statement;
    }

    public virtual string Name => Statement.GetType().Name.Replace("Statement", "");
    public virtual string Description => Name;
    public virtual IEnumerable<IStatementNavigator> GetChildren() => Enumerable.Empty<IStatementNavigator>();
}