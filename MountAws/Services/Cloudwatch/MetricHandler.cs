using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;
using MountAnything;

namespace MountAws.Services.Cloudwatch;

public class MetricHandler : PathHandler
{
    private readonly MetricName _metricName;
    private readonly IAmazonCloudWatch _cloudwatch;
    private readonly MetricNavigator _navigator;

    public MetricHandler(ItemPath path, IPathHandlerContext context, IAmazonCloudWatch cloudWatch, MetricNavigator navigator, MetricName metricName) : base(path, context)
    {
        _cloudwatch = cloudWatch;
        _navigator = navigator;
        _metricName = metricName;
    }
    private ItemPath MetricPath => _metricName.NamespaceAndName;

    protected override IItem? GetItemImpl()
    {
        var metric = MetricPath.Parent.IsRoot ? null : _cloudwatch.GetMetricOrDefault(MetricPath.Parent.FullName, MetricPath.Name);

        if (metric == null)
        {
            if (!MetricPath.Parent.IsRoot && !MetricPath.Parent.Parent.IsRoot)
            {
                return GetDimensionalMetricItem(MetricPath.Parent.Parent, MetricPath.Parent.Name);
            }

            return new MetricItem(ParentPath, MetricPath);
        }

        return new MetricItem(ParentPath, metric);
    }

    private DimensionalMetricItem? GetDimensionalMetricItem(ItemPath metricPath, string schemaItemName)
    {
        var @namespace = metricPath.Parent;
        var metricName = metricPath.Name;
        var dimensionNames = schemaItemName.Split(".");
        var dimensionValues = ItemName.Split(".");
        var dimensionsToMatch = dimensionNames.Select((name, index) => new Dimension
        {
            Name = name,
            Value = dimensionValues[index]
        });

        var metric = _cloudwatch.ListMetrics(@namespace, metricName)
            .MatchingDimensionsOrDefault(dimensionsToMatch);

        return metric != null
            ? new DimensionalMetricItem(ParentPath, @namespace.FullName, metricName, metric.Dimensions)
            : null;
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return GetItem() switch
        {
            MetricItem { ItemType: CloudwatchItemTypes.Directory } => GetChildMetricsWithinNamespace(),
            MetricItem { ItemType: CloudwatchItemTypes.Metric } metricItem => GetMetricChildren(metricItem),
            DimensionalMetricItem => GetDimensionalMetricChildren(),
            _ => Enumerable.Empty<IItem>()
        };
    }

    private IEnumerable<IItem> GetDimensionalMetricChildren()
    {
        return MetricTimeframe.All.Select(t => new MetricTimeframeItem(Path, t));
    }

    private IEnumerable<IItem> GetMetricChildren(MetricItem item)
    {
        return _cloudwatch.ListMetrics(new ItemPath(item.Namespace), item.MetricName)
            .Select(m => new DimensionalMetricItem(Path, m.Namespace, m.MetricName, m.Dimensions))
            .OrderBy(i => i.ItemName);
    }

    private IEnumerable<IItem> GetChildMetricsWithinNamespace()
    {
        WriteDebug($"ListChildItems({Path}, {MetricPath})");
        return _navigator.ListChildItems(Path, MetricPath);
    }
}