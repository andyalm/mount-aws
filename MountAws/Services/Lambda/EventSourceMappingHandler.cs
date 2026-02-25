using Amazon.Lambda;
using Amazon.Lambda.Model;
using MountAnything;

namespace MountAws.Services.Lambda;

public class EventSourceMappingHandler : PathHandler
{
    private readonly IAmazonLambda _lambda;

    public EventSourceMappingHandler(ItemPath path, IPathHandlerContext context,
        IAmazonLambda lambda)
        : base(path, context)
    {
        _lambda = lambda;
    }

    protected override IItem? GetItemImpl()
    {
        try
        {
            var mapping = _lambda.GetEventSourceMapping(ItemName);
            return new EventSourceMappingItem(ParentPath, mapping);
        }
        catch (ResourceNotFoundException)
        {
            return null;
        }
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield break;
    }
}
