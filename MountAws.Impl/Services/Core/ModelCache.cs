using System.Collections.Concurrent;

namespace MountAws.Services.Core;

public class ModelCache
{
    private ConcurrentDictionary<string, CachedItem> _cache = new();

    public TModel GetOrFetch<TModel>(string key, Func<TModel> fetch)
    {
        if (_cache.TryGetValue(key, out var cachedItem) && !cachedItem.IsExpired && cachedItem.Value is TModel cachedModel)
        {
            return cachedModel;
        }

        var fromSource = fetch()!;
        cachedItem = new CachedItem(fromSource, DateTimeOffset.MaxValue);
        _cache[key] = cachedItem;

        return fromSource;
    }

    private record CachedItem(object Value, DateTimeOffset Expiration)
    {
        public bool IsExpired => DateTimeOffset.UtcNow > Expiration;
    }
}