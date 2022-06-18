using Amazon.CloudWatch;
using MountAnything;

namespace MountAws.Services.Cloudwatch;

public class MetricAggregationHandler : PathHandler
{
    private readonly IItemAncestor<DimensionalMetricItem> _metric;
    private readonly IItemAncestor<MetricTimeframeItem> _timeframe;
    private readonly IAmazonCloudWatch _cloudwatch;

    public MetricAggregationHandler(ItemPath path, IPathHandlerContext context,
        IItemAncestor<DimensionalMetricItem> metric,
        IItemAncestor<MetricTimeframeItem> timeframe,
        IAmazonCloudWatch cloudwatch) : base(path, context)
    {
        _metric = metric;
        _timeframe = timeframe;
        _cloudwatch = cloudwatch;
    }

    protected override IItem? GetItemImpl()
    {
        if (MetricAggregation.TryParse(ItemName, out var aggregation))
        {
            return new MetricAggregationItem(ParentPath, aggregation);
        }

        return null;
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        if (GetItem() is MetricAggregationItem aggregation)
        {
            return _cloudwatch.GetMetricStatistics(_metric.Item.Namespace,
                _metric.Item.MetricName,
                _metric.Item.Dimensions,
                _timeframe.Item.UnderlyingObject,
                aggregation.UnderlyingObject).Select(s => new DatapointItem(Path, s, aggregation.UnderlyingObject));
        }

        return Enumerable.Empty<IItem>();
    }
}