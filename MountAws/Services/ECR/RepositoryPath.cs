namespace MountAws.Services.ECR;

public class RepositoryPath
{
    public string Value { get; }
    
    public RepositoryPath(string value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Value;
    }
}