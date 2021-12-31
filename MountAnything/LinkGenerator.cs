namespace MountAnything;

public class LinkGenerator
{
    public ItemPath BasePath { get; }

    public LinkGenerator(ItemPath basePath)
    {
        BasePath = basePath;
    }
    
    public ItemPath ConstructPath(int numberOfParentPathParts, string childPath)
    {
        var parts = BasePath.Parts;
        if (parts.Length < numberOfParentPathParts)
        {
            throw new InvalidOperationException(
                $"There is not enough context in the current path to construct a path to '{childPath}'");
        }

        var parentPath = new ItemPath(string.Join(ItemPath.Separator, parts[..numberOfParentPathParts]));

        return parentPath.Combine(childPath);
    }
}