namespace MountAnything;

public interface IPathHandler
{
    string Path { get; }
    bool Exists();
    IItem? GetItem();
    IEnumerable<IItem> GetChildItems(bool useCache = false);
    IEnumerable<IItem> GetChildItems(string filter);
}