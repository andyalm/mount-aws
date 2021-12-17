namespace MountAnything;

public class LinkGenerator
{
    public string BasePath { get; }

    public LinkGenerator(string basePath)
    {
        BasePath = basePath;
    }
    
    public string ConstructPath(int numberOfParentPathParts, string childPath)
    {
        var parts = BasePath.Split(ItemPath.Separator);
        if (parts.Length < numberOfParentPathParts)
        {
            throw new InvalidOperationException(
                $"There is not enough context in the current path to construct a path to '{childPath}'");
        }

        var parentPath = string.Join(ItemPath.Separator, parts[..numberOfParentPathParts]);

        return ItemPath.Combine(parentPath, childPath);
    }
}