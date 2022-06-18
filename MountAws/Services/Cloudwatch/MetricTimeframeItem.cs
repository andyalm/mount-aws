using MountAnything;

namespace MountAws.Services.Cloudwatch;

public class MetricTimeframeItem : AwsItem<MetricTimeframe>
{
    public MetricTimeframeItem(ItemPath parentPath, MetricTimeframe timeframe) : base(parentPath, timeframe)
    {
        ItemName = timeframe.Name;
    }

    public override string ItemName { get; }
    public override bool IsContainer => true;
}