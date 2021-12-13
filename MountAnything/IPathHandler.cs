namespace MountAnything;

public interface IPathHandler
{
    string Path { get; }
    bool Exists();
    Item? GetItem();
    IEnumerable<Item> GetChildItems(bool useCache = false);
    IEnumerable<string> ExpandPath(string path);
}