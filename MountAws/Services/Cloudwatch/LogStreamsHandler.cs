using Amazon.CloudWatchLogs;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Cloudwatch;

public class LogStreamsHandler : PathHandler
{
    private readonly IPathHandlerContext _context;
    private readonly IAmazonCloudWatchLogs _logs;
    private readonly IItemAncestor<LogGroupItem> _logGroup;

    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "streams", "Navigate the log streams of this log group");
    }
    
    private readonly Lazy<LogStreamNavigator> _navigator;

    public LogStreamsHandler(ItemPath path, IPathHandlerContext context, IAmazonCloudWatchLogs logs, IItemAncestor<LogGroupItem> logGroup) : base(path, context)
    {
        _context = context;
        _logs = logs;
        _logGroup = logGroup;
        _navigator = new Lazy<LogStreamNavigator>(CreateNavigator);
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return _navigator.Value.ListChildItems(Path);
    }

    private LogStreamNavigator CreateNavigator()
    {
        return new LogStreamNavigator(_logs, _context, _logGroup.Item.LogGroupName!);
    }
}