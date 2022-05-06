using MountAnything;

namespace MountAws.Services.Iam;

public record IamItemPath(ItemPath Path)
{
    public static IamItemPath Parse(string value)
    {
        return new IamItemPath(new ItemPath(value));
    }
    
    public string Name => Path.Name;

    public ItemPath Parent => Path.Parent;
    
    public override string ToString()
    {
        return Path.FullName;
    }
}