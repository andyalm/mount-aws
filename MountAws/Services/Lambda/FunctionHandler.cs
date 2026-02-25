using Amazon.Lambda;
using Amazon.Lambda.Model;
using MountAnything;

namespace MountAws.Services.Lambda;

public class FunctionHandler : PathHandler
{
    private readonly IAmazonLambda _lambda;

    public FunctionHandler(ItemPath path, IPathHandlerContext context, IAmazonLambda lambda)
        : base(path, context)
    {
        _lambda = lambda;
    }

    protected override IItem? GetItemImpl()
    {
        try
        {
            var function = _lambda.GetFunction(ItemName);
            return new FunctionItem(ParentPath, function);
        }
        catch (ResourceNotFoundException)
        {
            return null;
        }
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield return AliasesHandler.CreateItem(Path);
        yield return VersionsHandler.CreateItem(Path);
        yield return EventSourceMappingsHandler.CreateItem(Path);
    }
}
