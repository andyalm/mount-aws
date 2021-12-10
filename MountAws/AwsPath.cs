namespace MountAws;

public class AwsPath
{
    public static string Normalize(string path)
    {
        var normalizedPath = path.Replace(@"\", "/");
        if (normalizedPath.StartsWith("/"))
        {
            return normalizedPath.Substring(1);
        }

        return normalizedPath;
    }
    
    public static string GetParent(string path)
    {
        return Path.GetDirectoryName(path)!.Replace(@"\", "/");
    }

    public static string GetLeaf(string path)
    {
        return Path.GetFileName(path);
    }

    public static string Combine(params string[] parts)
    {
        return Path.Combine(parts).Replace(@"\", "/");
    }
}