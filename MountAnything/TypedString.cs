namespace MountAnything;

public abstract record TypedString(string Value)
{
    public static implicit operator string(TypedString typedString) => typedString.Value;

    public override string ToString()
    {
        return Value;
    }
}