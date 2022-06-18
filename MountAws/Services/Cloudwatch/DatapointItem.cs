using System.Management.Automation;
using Amazon.CloudWatch.Model;
using MountAnything;

namespace MountAws.Services.Cloudwatch;

public class DatapointItem : AwsItem
{
    public DatapointItem(ItemPath parentPath, Datapoint datapoint, MetricAggregation aggregation) : base(parentPath, new PSObject())
    {
        ItemName = datapoint.Timestamp.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss") + "Z";
        Timestamp = datapoint.Timestamp;
        Value = aggregation.GetValue(datapoint);
    }

    public override string ItemName { get; }

    [ItemProperty]
    public DateTime Timestamp { get; }
    
    [ItemProperty]
    public double Value { get; }

    public override bool IsContainer => false;
}