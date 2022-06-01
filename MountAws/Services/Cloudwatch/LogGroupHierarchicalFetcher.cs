using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using MountAnything;

namespace MountAws.Services.Cloudwatch;

public class LogGroupHierarchicalFetcher : HierarchicalItemFetcher<LogGroup,LogGroupItem>
{
    private readonly IAmazonCloudWatchLogs _logs;
    private readonly IPathHandlerContext _context;

    public LogGroupHierarchicalFetcher(IAmazonCloudWatchLogs logs, IPathHandlerContext context)
    {
        _logs = logs;
        _context = context;
    }

    protected override LogGroupItem CreateDirectoryItem(ItemPath parentPath, ItemPath directoryPath)
    {
        return new LogGroupItem(parentPath, directoryPath);
    }

    protected override LogGroupItem CreateItem(ItemPath parentPath, LogGroup model)
    {
        return new LogGroupItem(parentPath, model);
    }

    protected override ItemPath GetPath(LogGroup model)
    {
        return new ItemPath(model.LogGroupName);
    }

    protected override IEnumerable<LogGroup> ListItems(ItemPath? pathPrefix)
    {
        return _logs.DescribeLogGroups(pathPrefix);
    }
}