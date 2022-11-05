using System.Management.Automation;
using Amazon.CloudWatchLogs.Model;
using MountAnything;

namespace MountAws.Services.Cloudwatch;

public class LogGroupItem : AwsItem
{
    public LogGroupItem(ItemPath parentPath, ItemPath logGroupPath) : base(parentPath, new PSObject(new
    {
        Path = logGroupPath
    }))
    {
        ItemName = logGroupPath.Name;
        ItemType = CloudwatchItemTypes.Directory;
    }

    public LogGroupItem(ItemPath parentPath, LogGroup logGroup, string itemName) : base(parentPath, new PSObject(logGroup))
    {
        ItemName = itemName;
        ItemType = CloudwatchItemTypes.LogGroup;
        LogGroupName = logGroup.LogGroupName;
        WebUrl = UrlBuilder.CombineWith(
            $"cloudwatch/home?#logsV2:log-groups/log-group/{logGroup.LogGroupName.Replace("/", "$252F")}");
    }

    public override string ItemName { get; }
    public string? LogGroupName { get; }
    public override string ItemType { get; }
    public override bool IsContainer => true;
    public override string? WebUrl { get; }
}