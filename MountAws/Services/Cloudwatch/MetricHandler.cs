using Amazon.CloudWatch;
using MountAnything;

namespace MountAws.Services.Cloudwatch;

public class MetricNamespaceHandler : PathHandler
{
    private readonly MetricName _metricName;
    private readonly IAmazonCloudWatch _cloudwatch;
    private readonly MetricNavigator _navigator;

    public MetricNamespaceHandler(ItemPath path, IPathHandlerContext context, IAmazonCloudWatch cloudWatch, MetricNavigator navigator, MetricName metricName) : base(path, context)
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
            return new MetricItem(ParentPath, MetricPath);
        }
        
        return new MetricItem(ParentPath, metric);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return GetItem() switch
        {
            MetricItem { ItemType: CloudwatchItemTypes.Directory } => GetChildMetricsWithinNamespace(),
            MetricItem { ItemType: CloudwatchItemTypes.Metric } => GetMetricChildren(),
            _ => Enumerable.Empty<IItem>()
        };
    }

    private IEnumerable<IItem> GetMetricChildren()
    {
        WriteDebug("GetMetricChildren");
        yield break;
    }

    private IEnumerable<IItem> GetChildMetricsWithinNamespace()
    {
        WriteDebug($"ListChildItems({Path}, {MetricPath})");
        return _navigator.ListChildItems(Path, MetricPath);
    }
}