namespace MountAws;

public interface IPathHandler
{
    string Path { get; }
    bool Exists();
    AwsItem? GetItem();
    IEnumerable<AwsItem> GetChildItems(bool useCache = false);
}