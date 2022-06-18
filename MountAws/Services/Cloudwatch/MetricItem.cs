using System.Collections;
using System.Management.Automation;
using Amazon.CloudWatch.Model;
using MountAnything;

namespace MountAws.Services.Cloudwatch;

public class MetricItem : AwsItem
{
    public MetricItem(ItemPath parentPath, ItemPath namespacePath) : base(parentPath, new PSObject())
    {
        ItemName = namespacePath.Name;
        ItemType = CloudwatchItemTypes.Directory;
        Namespace = namespacePath.ToString();
    }
    
    public MetricItem(ItemPath parentPath, IGrouping<(string Namespace, string MetricName),Metric> metrics) : base(parentPath, new PSObject())
    {
        ItemName = metrics.Key.MetricName;
        ItemType = CloudwatchItemTypes.Metric;
        Dimensions = metrics.DimensionNames();
        MetricName = metrics.Key.MetricName;
        Namespace = metrics.Key.Namespace;
    }

    [ItemProperty]
    public string[][]? Dimensions { get; }
    
    [ItemProperty]
    public string Namespace { get; }
    
    [ItemProperty]
    public string? MetricName { get; }
    public override string ItemName { get; }
    public override string ItemType { get; }
    public override bool IsContainer => true;
}