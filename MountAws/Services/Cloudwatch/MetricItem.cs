using System.Management.Automation;
using Amazon.CloudWatch.Model;
using MountAnything;

namespace MountAws.Services.Cloudwatch;

public class MetricItem : AwsItem
{
    public MetricItem(ItemPath parentPath, ItemPath namespacePath) : base(parentPath, new PSObject(new
    {
        Namespace = namespacePath.ToString()
    }))
    {
        ItemName = namespacePath.Name;
        ItemType = CloudwatchItemTypes.Directory;
    }
    
    public MetricItem(ItemPath parentPath, Metric metric) : base(parentPath, new PSObject(metric))
    {
        ItemName = metric.MetricName;
        ItemType = CloudwatchItemTypes.Metric;
    }

    public override string ItemName { get; }
    public override string ItemType { get; }
    public override bool IsContainer => true;
}