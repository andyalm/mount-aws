using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;
using MountAnything;

namespace MountAws.Services.Cloudwatch;

public class MetricNavigator : ItemNavigator<IGrouping<(string Namespace, string MetricName),Metric>, MetricItem>
{
    private readonly IAmazonCloudWatch _cloudwatch;

    public MetricNavigator(IAmazonCloudWatch cloudwatch)
    {
        _cloudwatch = cloudwatch;
    }

    protected override MetricItem CreateDirectoryItem(ItemPath parentPath, ItemPath directoryPath)
    {
        return new MetricItem(parentPath, directoryPath);
    }

    protected override MetricItem CreateItem(ItemPath parentPath, IGrouping<(string Namespace, string MetricName),Metric> model)
    {
        return new MetricItem(parentPath, model);
    }

    protected override ItemPath GetPath(IGrouping<(string Namespace, string MetricName),Metric> model)
    {
        return new ItemPath(model.Key.Namespace).Combine(model.Key.MetricName);
    }

    protected override IEnumerable<IGrouping<(string Namespace, string MetricName),Metric>> ListItems(ItemPath? pathPrefix)
    {
        var metrics = _cloudwatch.ListMetrics(pathPrefix).ToArray();
        //pathPrefix could be a partial namespace, which the CW api doesn't support filtering by, so if we get no results back, query with no namespace
        if (!metrics.Any())
        {
            metrics = _cloudwatch.ListMetrics().ToArray();
        }

        return metrics.GroupBy(m => (m.Namespace,m.MetricName));
    }
}