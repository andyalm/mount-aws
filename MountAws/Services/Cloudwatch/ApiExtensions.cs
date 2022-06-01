using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using MountAnything;

using static MountAws.PagingHelper;

namespace MountAws.Services.Cloudwatch;

public static class ApiExtensions
{
    public static LogGroup? GetLogGroupOrDefault(this IAmazonCloudWatchLogs logs, ItemPath itemPath)
    {
        var logGroupPrefix = $"/{itemPath}";
        var matchingLogGroup = logs.DescribeLogGroupsAsync(new DescribeLogGroupsRequest
        {
            Limit = 1,
            LogGroupNamePrefix = logGroupPrefix
        }).GetAwaiter().GetResult().LogGroups.SingleOrDefault();
        if (matchingLogGroup?.LogGroupName.Equals(logGroupPrefix, StringComparison.OrdinalIgnoreCase) == true)
        {
            return matchingLogGroup;
        }

        return null;
    }

    public static IEnumerable<LogGroup> DescribeLogGroups(this IAmazonCloudWatchLogs logs, ItemPath? pathPrefix)
    {
        return Paginate(nextToken =>
        {
            var response = logs.DescribeLogGroupsAsync(new DescribeLogGroupsRequest
            {
                LogGroupNamePrefix = pathPrefix != null ? $"/{pathPrefix}/" : null,
                NextToken = nextToken
            }).GetAwaiter().GetResult();

            return (response.LogGroups, response.NextToken);
        });
    }
}