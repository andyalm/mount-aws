using MountAnything;

namespace MountAws.Services.Cloudwatch;

public record MetricName(ItemPath NamespaceAndName)
{
    public static LogStreamPath Parse(string value)
    {
        return new LogStreamPath(new ItemPath(value));
    }
    
    public string Name => NamespaceAndName.Name;

    public ItemPath Parent => NamespaceAndName.Parent;
    
    public override string ToString()
    {
        return NamespaceAndName.ToString();
    }
}