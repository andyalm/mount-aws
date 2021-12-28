namespace MountAnything;

public abstract class Freshness
{
    /// <summary>
    /// Default cache behavior. Will use the cache unless the force flag has been set
    /// </summary>
    public static Freshness Default { get; } = new DefaultFreshness();
    
    /// <summary>
    /// Guarantees a fresh version of the item is returned. Cached items will be ignored.
    /// </summary>
    public static Freshness Guaranteed { get; } = new GuaranteedFreshness();
    
    /// <summary>
    /// Optimize for performance and always use a cached value if available, regardless of force flag.
    /// This is used in contexts such as tab completion/path expansion where speed is important and
    /// we can generally live with a missing item or stale state.
    /// </summary>
    public static Freshness Fastest { get; } = new FastestFreshness();

    public abstract bool IsFresh(DateTimeOffset cachedTimestamp, bool force);
    
    private class DefaultFreshness : Freshness
    {
        public override bool IsFresh(DateTimeOffset cachedTimestamp, bool force) =>
            !force;
    }
    
    private class GuaranteedFreshness : Freshness
    {
        public override bool IsFresh(DateTimeOffset cachedTimestamp, bool force) =>
            !force && cachedTimestamp.AddSeconds(15) > DateTimeOffset.UtcNow;
    }
    
    private class FastestFreshness : Freshness
    {
        public override bool IsFresh(DateTimeOffset cachedTimestamp, bool force) => 
            cachedTimestamp.AddHours(4) > DateTimeOffset.UtcNow;
    }
}