using Amazon.CloudWatchLogs;
using MountAnything;

namespace MountAws.Services.Cloudwatch;

public class LogGroupHandler : LogGroupHandlerBase
{
    private readonly LogGroupName _logGroupPath;

    public LogGroupHandler(ItemPath path, IPathHandlerContext context, IAmazonCloudWatchLogs logs, LogGroupName logGroupPath) : base(path, context, logs)
    {
        _logGroupPath = logGroupPath;
    }

    protected override string LogGroupPath => _logGroupPath.Path.ToString();
}