using System.Management.Automation;
using Amazon.CloudWatchLogs.Model;
using MountAnything;

namespace MountAws.Services.Cloudwatch;

public class LogStreamItem : AwsItem
{
    public LogStreamItem(ItemPath parentPath, LogStream stream, string logGroupName) : base(parentPath, new PSObject(stream))
    {
        ItemName = new ItemPath(stream.LogStreamName).Name;
        ItemType = CloudwatchItemTypes.LogStream;
        LogStreamName = stream.LogStreamName;
        WebUrl = UrlBuilder.CombineWith(
            $"cloudwatch/home?#logsV2:log-groups/log-group/{logGroupName.Replace("/", "$252F")}/log-events/{stream.LogStreamName.Replace("/", "$252F")}");
    }
    
    public LogStreamItem(ItemPath parentPath, ItemPath streamPath) : base(parentPath, new PSObject(new
    {
        Path = streamPath.ToString()
    }))
    {
        ItemName = streamPath.Name;
        ItemType = CloudwatchItemTypes.Directory;
    }

    public string? LogStreamName { get; }
    public override string ItemName { get; }
    public override string ItemType { get; }
    public override bool IsContainer => true;
    public override string? WebUrl { get; }
}