using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;
using MountAnything;

namespace MountAws.Services.Cloudwatch;

public class MetricNavigator : ItemNavigator<Metric, MetricItem>
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

    protected override MetricItem CreateItem(ItemPath parentPath, Metric model)
    {
        return new MetricItem(parentPath, model);
    }

    protected override ItemPath GetPath(Metric model)
    {
        return new ItemPath(model.Namespace).Combine(model.MetricName);
    }

    protected override IEnumerable<Metric> ListItems(ItemPath? pathPrefix)
    {
        var metrics = _cloudwatch.ListMetrics(pathPrefix).ToArray();
        //pathPrefix could be a partial namespace, which the CW api doesn't support filtering by, so if we get no results back, query with no namespace
        if (!metrics.Any())
        {
            return _cloudwatch.ListMetrics();
        }

        return metrics;
    }
}