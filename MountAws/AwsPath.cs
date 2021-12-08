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
}