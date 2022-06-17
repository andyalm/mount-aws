using System.Management.Automation;
using MountAnything;

namespace MountAws.Services.Cloudwatch;

public class MetricDimensionItem : AwsItem
{
    public MetricDimensionItem(ItemPath parentPath, string @namespace, string metricName, string[] dimensions) : base(parentPath, new PSObject())
    {
        Namespace = @namespace;
        MetricName = metricName;
        Dimensions = dimensions;
        ItemName = dimensions.DimensionItemName();
    }
    
    [ItemProperty]
    public string[] Dimensions { get; }
    
    [ItemProperty]
    public string Namespace { get; }
    
    [ItemProperty]
    public string MetricName { get; }
    public override string ItemName { get; }

    public override bool IsContainer => true;
}