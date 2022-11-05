using MountAnything;

namespace MountAws.Services.Cloudwatch;

public class MetricTimeframeHandler : PathHandler
{
    public MetricTimeframeHandler(ItemPath path, IPathHandlerContext context) : base(path, context)
    {
    }

    protected override IItem? GetItemImpl()
    {
        if (MetricTimeframe.TryFromName(ItemName, out var timeframe))
        {
            return new MetricTimeframeItem(ParentPath, timeframe);
        }

        return null;
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return MetricAggregation.All.Select(a => new MetricAggregationItem(Path, a));
    }
}