using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using MountAnything;

namespace MountAws.Services.Cloudwatch;

public class LogGroupNavigator : ItemNavigator<LogGroup,LogGroupItem>
{
    private readonly IAmazonCloudWatchLogs _logs;

    public LogGroupNavigator(IAmazonCloudWatchLogs logs)
    {
        _logs = logs;
    }

    protected override LogGroupItem CreateDirectoryItem(ItemPath parentPath, ItemPath directoryPath)
    {
        return new LogGroupItem(parentPath, directoryPath);
    }

    protected override LogGroupItem CreateItem(ItemPath parentPath, LogGroup model)
    {
        return new LogGroupItem(parentPath, model, model.LogGroupName.Split("/").Last());
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