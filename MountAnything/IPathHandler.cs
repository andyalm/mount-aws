namespace MountAnything;

public interface IPathHandler
{
    string Path { get; }
    bool Exists();
    IItem? GetItem(Freshness? freshness = null);
    Freshness GetItemCommandDefaultFreshness { get; }
    IEnumerable<IItem> GetChildItems(Freshness? freshness = null);
    Freshness GetChildItemsCommandDefaultFreshness { get; }
    IEnumerable<IItem> GetChildItems(string filter);
}