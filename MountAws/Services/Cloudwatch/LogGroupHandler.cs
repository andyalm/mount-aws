using Amazon.CloudWatchLogs;
using MountAnything;

namespace MountAws.Services.Cloudwatch;

public class LogGroupHandler : PathHandler
{
    private readonly LogGroupHierarchicalFetcher _hierarchicalFetcher;
    private readonly IAmazonCloudWatchLogs _logs;
    private readonly LogGroupName _logGroupPath;

    public LogGroupHandler(ItemPath path, IPathHandlerContext context, LogGroupHierarchicalFetcher hierarchicalFetcher, IAmazonCloudWatchLogs logs, LogGroupName logGroupPath) : base(path, context)
    {
        _hierarchicalFetcher = hierarchicalFetcher;
        _logs = logs;
        _logGroupPath = logGroupPath;
    }

    protected override IItem? GetItemImpl()
    {
        var logGroup = _logs.GetLogGroupOrDefault(_logGroupPath.Path);
        if (logGroup != null)
        {
            return new LogGroupItem(ParentPath, logGroup);
        }

        return new LogGroupItem(ParentPath, _logGroupPath.Path);
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
        yield break;
    }

    private IEnumerable<IItem> GetChildLogGroupsWithinDirectory()
    {
        return _hierarchicalFetcher.ListChildItems(Path, _logGroupPath.Path);
    }
}