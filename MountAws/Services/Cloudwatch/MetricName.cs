using MountAnything;

namespace MountAws.Services.Cloudwatch;

public record MetricName(ItemPath NamespaceAndName)
{
    public static MetricName Parse(string value)
    {
        return new MetricName(new ItemPath(value));
    }

    public string Name => NamespaceAndName.Name;

    public ItemPath Parent => NamespaceAndName.Parent;
    
    public override string ToString()
    {
        return NamespaceAndName.ToString();
    }
}