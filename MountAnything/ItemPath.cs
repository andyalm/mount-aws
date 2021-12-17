namespace MountAnything;

public static class ItemPath
{
    public const char Separator = '/';
    
    public static string Normalize(string path)
    {
        var normalizedPath = path.Replace(@"\", Separator.ToString());
        if (normalizedPath.StartsWith(Separator))
        {
            return normalizedPath.Substring(1);
        }

        return normalizedPath;
    }
    
    public static string GetParent(string path)
    {
        return Path.GetDirectoryName(path)!.Replace(@"\", Separator.ToString());
    }

    public static string GetLeaf(string path)
    {
        return Path.GetFileName(path);
    }

    public static string Combine(params string[] parts)
    {
        return Path.Combine(parts).Replace(@"\", Separator.ToString());
    }
}