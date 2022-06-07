using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using MountAnything;

namespace MountAws.Services.Cloudwatch;

public class LogStreamNavigator : ItemNavigator<LogStream, LogStreamItem>
{
    private readonly IAmazonCloudWatchLogs _logs;
    private readonly IPathHandlerContext _context;
    private readonly string _logGroupName;

    public LogStreamNavigator(IAmazonCloudWatchLogs logs, IPathHandlerContext context, string logGroupName)
    {
        _logs = logs;
        _context = context;
        _logGroupName = logGroupName;
    }

    protected override LogStreamItem CreateDirectoryItem(ItemPath parentPath, ItemPath directoryPath)
    {
        return new LogStreamItem(parentPath, directoryPath);
    }

    protected override LogStreamItem CreateItem(ItemPath parentPath, LogStream model)
    {
        return new LogStreamItem(parentPath, model);
    }

    protected override ItemPath GetPath(LogStream model)
    {
        return new ItemPath(model.LogStreamName);
    }

    protected override IEnumerable<LogStream> ListItems(ItemPath? pathPrefix)
    {
        return _logs.DescribeLogStreams(_logGroupName, pathPrefix);
    }
}