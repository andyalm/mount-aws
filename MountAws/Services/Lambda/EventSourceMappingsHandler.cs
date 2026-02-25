using Amazon.Lambda;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Lambda;

public class EventSourceMappingsHandler : PathHandler
{
    private readonly IAmazonLambda _lambda;
    private readonly CurrentFunction _currentFunction;

    public static Item CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "event-source-mappings",
            "Navigate the event source mappings for this Lambda function");
    }

    public EventSourceMappingsHandler(ItemPath path, IPathHandlerContext context,
        IAmazonLambda lambda, CurrentFunction currentFunction)
        : base(path, context)
    {
        _lambda = lambda;
        _currentFunction = currentFunction;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return _lambda.ListEventSourceMappings(_currentFunction.Name)
            .Select(m => new EventSourceMappingItem(Path, m))
            .OrderBy(m => m.ItemName);
    }
}
