namespace MountAws.Services.SecretsManager;

public class SecretPath
{
    public string Value { get; }

    public SecretPath(string value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Value;
    }
}
