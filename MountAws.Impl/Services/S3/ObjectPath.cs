namespace MountAws.Services.S3;

public class ObjectPath
{
    public string Value { get; }

    public ObjectPath(string value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Value;
    }
}