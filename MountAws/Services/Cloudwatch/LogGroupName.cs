using MountAnything;

namespace MountAws.Services.Cloudwatch;

public record LogGroupName(ItemPath Path)
{
    public static LogGroupName Parse(string value)
    {
        return new LogGroupName(new ItemPath(value));
    }
    
    public string Name => Path.Name;

    public ItemPath Parent => Path.Parent;
    
    public override string ToString()
    {
        return Path.FullName;
    }
}