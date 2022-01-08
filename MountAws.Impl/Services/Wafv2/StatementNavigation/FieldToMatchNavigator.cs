using Amazon.WAFV2.Model;

namespace MountAws.Services.Wafv2.StatementNavigation;

public class FieldToMatchNavigator : IStatementNavigator
{
    private readonly object _fieldToMatchObject;

    public FieldToMatchNavigator(object fieldToMatchObject)
    {
        _fieldToMatchObject = fieldToMatchObject;
    }

    public virtual string Name => _fieldToMatchObject.GetType().Name;
    public virtual string Description => Name;

    public virtual IEnumerable<IStatementNavigator> GetChildren() => Enumerable.Empty<IStatementNavigator>();
}

public class SingleHeaderNavigator : FieldToMatchNavigator
{
    private readonly SingleHeader _singleHeader;
    
    public SingleHeaderNavigator(SingleHeader singleHeader) : base(singleHeader)
    {
        _singleHeader = singleHeader;
    }

    public override string Name => _singleHeader.Name;
}