using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;
using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using MountAnything;

using static MountAws.PagingHelper;
using ResourceNotFoundException = Amazon.CloudWatchLogs.Model.ResourceNotFoundException;

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
        }, maxPages:10);
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
        return Paginate(nextToken =>
        {
            var response = logs.GetLogEventsAsync(new GetLogEventsRequest
            {
                LogGroupName = logGroupName,
                LogStreamName = logStreamName
            }).GetAwaiter().GetResult();

            return (response.Events, nextToken == response.NextForwardToken ? null : response.NextForwardToken);
        }, maxPages: 10);
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

    public static IEnumerable<Metric> ListMetrics(this IAmazonCloudWatch cloudWatch, ItemPath? namespacePath = null)
    {
        return Paginate(nextToken =>
        {
            var response = cloudWatch.ListMetricsAsync(new ListMetricsRequest
            {
                Namespace = namespacePath?.ToString(),
                NextToken = nextToken
            }).GetAwaiter().GetResult();

            return (response.Metrics, response.NextToken);
        }, maxPages:10);
    }

    public static Metric? GetMetricOrDefault(this IAmazonCloudWatch cloudwatch, string metricName)
    {
        return cloudwatch.ListMetricsAsync(new ListMetricsRequest
        {
            MetricName = metricName
        }).GetAwaiter().GetResult().Metrics.FirstOrDefault();
    }
}