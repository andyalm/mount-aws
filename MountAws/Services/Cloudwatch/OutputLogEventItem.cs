using Amazon.CloudWatchLogs.Model;
using MountAnything;

namespace MountAws.Services.Cloudwatch;

public class OutputLogEventItem : AwsItem<OutputLogEvent>
{
    public OutputLogEventItem(ItemPath parentPath, OutputLogEvent logEvent) : base(parentPath, logEvent)
    {
        ItemName = logEvent.Timestamp.ToUniversalTime().ToString("O");
    }

    public override string ItemName { get; }
    public override bool IsContainer => false;
}