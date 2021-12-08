namespace MountAws;

public class ProfileHandler : IPathHandler
{
    
    
    public string Path { get; }
    public bool Exists()
    {
        throw new NotImplementedException();
    }

    public AwsItem? GetItem()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<AwsItem> GetChildItems(bool useCache = false)
    {
        throw new NotImplementedException();
    }
}