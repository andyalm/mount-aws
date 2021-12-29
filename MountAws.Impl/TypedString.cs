namespace MountAws;

public abstract class TypedString
{
    public static implicit operator string(TypedString typedString) => typedString.Value;
    
    public string Value { get; private init; }

    public TypedString(string value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Value;
    }
}