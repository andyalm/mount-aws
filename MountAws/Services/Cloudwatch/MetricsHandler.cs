using Amazon.CloudWatch;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Cloudwatch;

public class MetricsHandler : PathHandler
{
    private readonly MetricNavigator _navigator;

    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "metrics",
            "Navigate cloudwatch metrics");
    }
    
    public MetricsHandler(ItemPath path, IPathHandlerContext context, MetricNavigator navigator) : base(path, context)
    {
        _navigator = navigator;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return _navigator.ListChildItems(Path);
    }
}