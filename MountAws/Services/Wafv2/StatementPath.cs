using MountAnything;

namespace MountAws.Services.Wafv2;

public record StatementPath
{
    public StatementPath(ItemPath itemPath)
    {
        Value = itemPath;
    }
    public StatementPath(string itemPath)
    {
        Value = new ItemPath(itemPath);
    }

    public bool IsRoot => Value.IsRoot;
    
    public ItemPath Value { get; }

    public override string ToString()
    {
        return Value.FullName;
    }
}