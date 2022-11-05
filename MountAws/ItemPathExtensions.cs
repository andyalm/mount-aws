using MountAnything;

namespace MountAws;

//TODO: Remove and use implementation from MountAnything
public static class ItemPathExtensions
{
    public static ItemPath SafeCombine(this ItemPath? path, string name)
    {
        if (path == null)
        {
            return new ItemPath(name);
        }

        return path.Combine(name);
    }
}