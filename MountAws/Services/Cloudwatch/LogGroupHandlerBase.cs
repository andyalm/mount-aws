using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using MountAnything;

namespace MountAws.Services.Cloudwatch;

public abstract class LogGroupHandlerBase : PathHandler
{
    private readonly LogGroupNavigator _navigator;
    private readonly IAmazonCloudWatchLogs _logs;

    public LogGroupHandlerBase(ItemPath path, IPathHandlerContext context, IAmazonCloudWatchLogs logs) : base(path, context)
    {
        _logs = logs;
        _navigator = new LogGroupNavigator(logs);
    }
    
    protected abstract string LogGroupPath { get; }

    protected override IItem? GetItemImpl()
    {
        var logGroupPath = new ItemPath(LogGroupPath);
        var logGroup = _logs.GetLogGroupOrDefault(logGroupPath);
        if (logGroup != null)
        {
            return new LogGroupItem(ParentPath, logGroup, ItemName);
        }

        return new LogGroupItem(ParentPath, logGroupPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return GetItem() switch
        {
            LogGroupItem { ItemType: CloudwatchItemTypes.Directory } => GetChildLogGroupsWithinDirectory(),
            LogGroupItem { ItemType: CloudwatchItemTypes.LogGroup } => GetLogGroupChildren(),
            _ => Enumerable.Empty<IItem>()
        };
    }

    private IEnumerable<IItem> GetLogGroupChildren()
    {
        yield return LogStreamsHandler.CreateItem(Path);
    }

    private IEnumerable<IItem> GetChildLogGroupsWithinDirectory()
    {
        return _navigator.ListChildItems(Path, new ItemPath(LogGroupPath));
    }
}