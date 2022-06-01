using System.Management.Automation;
using Amazon.CloudWatchLogs.Model;
using MountAnything;

namespace MountAws.Services.Cloudwatch;

public class LogGroupItem : AwsItem
{
    public LogGroupItem(ItemPath parentPath, ItemPath logGroupPath) : base(parentPath, new PSObject(new
    {
        Path = logGroupPath.ToString()
    }))
    {
        ItemName = logGroupPath.Name;
        ItemType = CloudwatchItemTypes.Directory;
    }

    public LogGroupItem(ItemPath parentPath, LogGroup logGroup) : base(parentPath, new PSObject(logGroup))
    {
        ItemName = logGroup.LogGroupName.Split("/").Last();
        ItemType = CloudwatchItemTypes.LogGroup;
    }

    public override string ItemName { get; }
    public override string ItemType { get; }
    public override bool IsContainer => true;
}