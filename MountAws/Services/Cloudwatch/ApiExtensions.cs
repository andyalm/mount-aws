using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using MountAnything;

using static MountAws.PagingHelper;

namespace MountAws.Services.Cloudwatch;

public static class ApiExtensions
{
    public static LogGroup? GetLogGroupOrDefault(this IAmazonCloudWatchLogs logs, ItemPath itemPath)
    {
        var logGroup = logs.GetLogGroupExactPathOrDefault(itemPath.FullName);
        if (logGroup == null)
        {
            logGroup = logs.GetLogGroupExactPathOrDefault($"/{itemPath}");
        }

        return logGroup;
    }
    
    private static LogGroup? GetLogGroupExactPathOrDefault(this IAmazonCloudWatchLogs logs, string path)
    {
        var request = new DescribeLogGroupsRequest
        {
            Limit = 1,
            LogGroupNamePrefix = path
        };
        var matchingLogGroup = logs.DescribeLogGroupsAsync(request).GetAwaiter().GetResult().LogGroups.SingleOrDefault();
        if (matchingLogGroup?.LogGroupName.Equals(path, StringComparison.OrdinalIgnoreCase) == true)
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

    public static IEnumerable<LogStream> DescribeLogStreams(this IAmazonCloudWatchLogs logs, string logGroupName, ItemPath? pathPrefix = null)
    {
        return Paginate(nextToken =>
        {
            var response = logs.DescribeLogStreamsAsync(new DescribeLogStreamsRequest
            {
                LogGroupName = logGroupName,
                LogStreamNamePrefix = pathPrefix != null && !pathPrefix.IsRoot ? pathPrefix.FullName : null,
                NextToken = nextToken
            }).GetAwaiter().GetResult();

            return (response.LogStreams, response.NextToken);
        }, maxPages:2);
    }
    
    public static LogStream? DescribeLogStreamOrDefault(this IAmazonCloudWatchLogs logs, string logGroupName, ItemPath pathPrefix)
    {
        try
        {
            var logStream = logs.DescribeLogStreamsAsync(new DescribeLogStreamsRequest
            {
                Limit = 1,
                LogGroupName = logGroupName,
                LogStreamNamePrefix = pathPrefix.IsRoot ? null : pathPrefix.FullName
            }).GetAwaiter().GetResult().LogStreams.FirstOrDefault();

            if (logStream?.LogStreamName.Equals(pathPrefix.FullName, StringComparison.OrdinalIgnoreCase) == true)
            {
                return logStream;
            }

            return null;
        }
        catch (ResourceNotFoundException)
        {
            return null;
        }
    }

    public static IEnumerable<OutputLogEvent> GetLogEvents(this IAmazonCloudWatchLogs logs, string logGroupName, string logStreamName)
    {
        return logs.GetLogEventsAsync(new GetLogEventsRequest
        {
            LogGroupName = logGroupName,
            LogStreamName = logStreamName
        }).GetAwaiter().GetResult().Events;
    }
    
    public static OutputLogEvent? GetLogEvent(this IAmazonCloudWatchLogs logs, string logGroupName, string logStreamName, DateTime messageDate)
    {
        return logs.GetLogEventsAsync(new GetLogEventsRequest
        {
            LogGroupName = logGroupName,
            LogStreamName = logStreamName,
            StartTime = messageDate,
            Limit = 1
        }).GetAwaiter().GetResult().Events.FirstOrDefault();
    }
}