using Amazon.CloudWatch;
using MountAnything;

namespace MountAws.Services.Cloudwatch;

public abstract class MetricHandlerBase : PathHandler
{
    private readonly IAmazonCloudWatch _cloudWatch;
    private readonly MetricNavigator _navigator;
    private readonly MetricName _metricName;

    protected MetricHandlerBase(ItemPath path, IPathHandlerContext context, IAmazonCloudWatch cloudWatch, MetricNavigator navigator, MetricName metricName) : base(path, context)
    {
        _cloudWatch = cloudWatch;
        _navigator = navigator;
        _metricName = metricName;
    }
    
    protected abstract string MetricName { get; }

    protected override IItem? GetItemImpl()
    {
        var metric = _cloudWatch.GetMetricOrDefault(MetricName);

        if (metric == null)
        {
            return new MetricItem(ParentPath, new ItemPath(ItemName));
        }
        
        return new MetricItem(ParentPath, _metricName.NamespaceAndName);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return GetItem() switch
        {
            LogGroupItem { ItemType: CloudwatchItemTypes.Directory } => GetChildMetricsWithinNamespace(),
            LogGroupItem { ItemType: CloudwatchItemTypes.LogGroup } => GetMetricChildren(),
            _ => Enumerable.Empty<IItem>()
        };
    }

    private IEnumerable<IItem> GetMetricChildren()
    {
        yield break;
    }

    private IEnumerable<IItem> GetChildMetricsWithinNamespace()
    {
        return _navigator.ListChildItems(Path, _metricName.NamespaceAndName);
    }
}