namespace MountAws.Services.Wafv2.StatementNavigation;

public abstract class StatementNavigator<TStatement> : IStatementNavigator where TStatement : class
{
    protected TStatement Statement { get; }

    protected StatementNavigator(TStatement statement, int position)
    {
        Statement = statement;
        Position = position;
    }

    public int Position { get; }
    public virtual string Name => Statement.GetType().Name.Replace("Statement", "").PascalToKebabCase()!;
    public virtual string Description => Name;
    object IStatementNavigator.UnderlyingObject => Statement;
    public virtual IEnumerable<IStatementNavigator> GetChildren() => Enumerable.Empty<IStatementNavigator>();
}