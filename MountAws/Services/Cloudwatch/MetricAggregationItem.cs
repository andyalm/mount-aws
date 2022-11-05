using MountAnything;

namespace MountAws.Services.Cloudwatch;

public class MetricAggregationItem : AwsItem<MetricAggregation>
{
    public MetricAggregationItem(ItemPath parentPath, MetricAggregation aggregation) : base(parentPath, aggregation)
    {
        ItemName = aggregation.Name;
    }

    public override string ItemName { get; }
    
    public override bool IsContainer => true;
}