using MountAnything;

namespace MountAws.Services.Cloudwatch;

public record LogStreamPath(ItemPath Path)
{
    public static LogStreamPath Parse(string value)
    {
        return new LogStreamPath(new ItemPath(value));
    }
    
    public string Name => Path.Name;

    public ItemPath Parent => Path.Parent;
    
    public override string ToString()
    {
        return Path.FullName;
    }
}