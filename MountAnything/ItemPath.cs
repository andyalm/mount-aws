namespace MountAnything;

public record struct ItemPath
{
    public static explicit operator string(ItemPath path) => path.FullName;
    public static explicit operator ItemPath(string value) => new(value);
    
    public const char Separator = '/';

    public static readonly ItemPath Root = new(string.Empty);
    public string FullName { get; }

    private readonly Lazy<ItemPath> _parent;
    private readonly Lazy<string[]> _parts;
    public ItemPath Parent => _parent.Value;
    public string Name { get; }

    public bool IsRoot => string.IsNullOrEmpty(FullName);

    public ItemPath(string fullName)
    {
        var normalizedPath = fullName.Replace(@"\", Separator.ToString());
        if (normalizedPath.StartsWith(Separator))
        {
            normalizedPath = normalizedPath.Substring(1);
        }

        FullName = normalizedPath;
        _parent = new Lazy<ItemPath>(() => new ItemPath(GetParent(normalizedPath)));
        _parts = new Lazy<string[]>(() => normalizedPath.Split(Separator));
        Name = Path.GetFileName(normalizedPath);
    }

    public string[] Parts => _parts.Value;

    public override string ToString()
    {
        return FullName;
    }

    private static string GetParent(string path)
    {
        return Path.GetDirectoryName(path)!.Replace(@"\", Separator.ToString());
    }

    public ItemPath Combine(params string[] parts)
    {
        var combinedPath = Path.Combine(new[] { FullName }.Concat(parts).ToArray()).Replace(@"\", Separator.ToString());
        return new ItemPath(combinedPath);
    }
}