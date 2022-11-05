using Amazon.CloudWatchLogs;
using MountAnything;

namespace MountAws.Services.Cloudwatch;

public class OutputLogEventHandler : PathHandler
{
    private readonly IAmazonCloudWatchLogs _logs;
    private readonly IItemAncestor<LogGroupItem> _logGroup;
    private readonly IItemAncestor<LogStreamItem> _stream;

    public OutputLogEventHandler(ItemPath path,
        IPathHandlerContext context,
        IAmazonCloudWatchLogs logs,
        IItemAncestor<LogGroupItem> logGroup,
        IItemAncestor<LogStreamItem> stream) : base(path, context)
    {
        _logs = logs;
        _logGroup = logGroup;
        _stream = stream;
    }

    protected override IItem? GetItemImpl()
    {
        if (DateTime.TryParse(ItemName, out var messageDate))
        {
            var logEvent = _logs.GetLogEvent(_logGroup.Item.LogGroupName!, _stream.Item.LogStreamName!, messageDate);
            if (logEvent != null)
            {
                return new OutputLogEventItem(ParentPath, logEvent);
            }
        }

        return null;
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield break;
    }
}