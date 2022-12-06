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
        Metric[] metrics;
        if (pathPrefix?.FullName == "AWS")
        {
            // this is a bit hacky, but there is no API to query for namespaces, so we use a hardcoded list for now
            // furthermor, the way this navigator class works, is it expects a concrete model to be returned and then it creates the hierarchy from the children
            // this, we create synthetic metrics that will just be used to find the namespaces
            metrics = SupportedNamespaces.Select(ns => new Metric
            {
                Namespace = $"AWS/{ns}",
                MetricName = "Unknown",
            }).ToArray();
        }
        else
        {
            metrics = _cloudwatch.ListMetrics(pathPrefix).ToArray();
        }

        return metrics.GroupBy(m => (m.Namespace,m.MetricName));
    }
    
    private static readonly string[] SupportedNamespaces =
    {
        "AmazonMQ",
        "ApiGateway",
        "ApplicationELB",
        "AutoScaling",
        "DynamoDB",
        "EBS",
        "EC2",
        "ECS",
        "EFS",
        "ELB",
        "ElastiCache",
        "NetworkELB",
        "RDS",
        "S3",
        "WAFV2"
    };
}