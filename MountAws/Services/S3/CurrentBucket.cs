namespace MountAws.Services.S3;

public class CurrentBucket
{
    public string Name { get; }

    public CurrentBucket(string name)
    {
        Name = name;
    }

    public override string ToString()
    {
        return Name;
    }
}