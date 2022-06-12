using Amazon.CloudWatch;
using MountAnything;

namespace MountAws.Services.Cloudwatch;

public class MetricHandler : MetricHandlerBase
{
    public MetricHandler(ItemPath path, IPathHandlerContext context, IAmazonCloudWatch cloudWatch, MetricNavigator navigator, MetricName metricName) : base(path, context, cloudWatch, navigator, metricName)
    {
    }

    protected override string MetricName => ItemName;
}