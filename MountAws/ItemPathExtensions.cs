using MountAnything;

namespace MountAws;

public static class ItemPathExtensions
{
    public static bool IsAncestorOf(this ItemPath path, ItemPath otherPath, out string? childPart)
    {
        if (path.IsRoot)
        {
            childPart = null;
            return false;
        }
        
        if (otherPath.IsRoot)
        {
            childPart = path.Parts.First();
            return true;
        }

        ItemPath currentPath = path.Parent;
        childPart = path.Name;
        while (!currentPath.IsRoot)
        {
            if (currentPath.FullName.Equals(otherPath.FullName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            childPart = currentPath.Name;
            currentPath = currentPath.Parent;
        }

        childPart = null;
        return false;
    }
    
    public static ItemPath SafeCombine(this ItemPath path, ItemPath? child)
    {
        if (child == null || child.IsRoot)
        {
            return path;
        }

        return path.Combine(child);
    }

    public static ItemPath SafeCombine(this ItemPath? path, string name)
    {
        if (path == null)
        {
            return new ItemPath(name);
        }

        return path.Combine(name);
    }
}