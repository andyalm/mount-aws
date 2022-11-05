using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using MountAnything;

namespace MountAws.Services.Cloudwatch;

public class LogStreamHandler : PathHandler
{
    private readonly IPathHandlerContext _context;
    private readonly IAmazonCloudWatchLogs _logs;
    private readonly Lazy<LogStreamNavigator> _navigator;
    private readonly IItemAncestor<LogGroupItem> _logGroup;
    private readonly LogStreamPath _logStreamPath;

    public LogStreamHandler(ItemPath path, IPathHandlerContext context,
        IAmazonCloudWatchLogs logs,
        IItemAncestor<LogGroupItem> logGroup,
        LogStreamPath logStreamPath) : base(path, context)
    {
        _context = context;
        _logs = logs;
        _logGroup = logGroup;
        _navigator = new Lazy<LogStreamNavigator>(CreateNavigator);
        _logStreamPath = logStreamPath;
    }

    protected override IItem? GetItemImpl()
    {
        var stream = _logs.DescribeLogStreamOrDefault(_logGroup.Item.LogGroupName!, _logStreamPath.Path);

        return stream != null ? new LogStreamItem(ParentPath, stream, _logGroup.Item.LogGroupName!) : new LogStreamItem(ParentPath, _logStreamPath.Path);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return GetItem() switch
        {
            LogStreamItem { ItemType: CloudwatchItemTypes.Directory } => GetChildLogStreamsWithinDirectory(),
            LogStreamItem { ItemType: CloudwatchItemTypes.LogStream } => GetLogStreamChildren(),
            _ => Enumerable.Empty<IItem>()
        };
    }

    private IEnumerable<IItem> GetChildLogStreamsWithinDirectory()
    {
        return _navigator.Value.ListChildItems(Path, _logStreamPath.Path);
    }

    private IEnumerable<IItem> GetLogStreamChildren()
    {
        return _logs.GetLogEvents(_logGroup.Item.LogGroupName!, _logStreamPath.Path.FullName)
            .Select(e => new OutputLogEventItem(Path, e));
    }

    private LogStreamNavigator CreateNavigator()
    {
        return new LogStreamNavigator(_logs, _context, _logGroup.Item.LogGroupName!);
    }
}