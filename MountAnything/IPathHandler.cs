namespace MountAnything;

public interface IPathHandler
{
    string Path { get; }
    bool Exists();
    IItem? GetItem(Freshness? freshness = null);
    Freshness GetItemDefaultFreshness { get; }
    IEnumerable<IItem> GetChildItems(Freshness? freshness = null);
    Freshness GetChildItemsDefaultFreshness { get; }
    IEnumerable<IItem> GetChildItems(string filter);
}