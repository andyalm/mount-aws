using Amazon.Lambda;
using Amazon.Lambda.Model;
using MountAnything;

namespace MountAws.Services.Lambda;

public class AliasHandler : PathHandler
{
    private readonly IAmazonLambda _lambda;
    private readonly CurrentFunction _currentFunction;

    public AliasHandler(ItemPath path, IPathHandlerContext context,
        IAmazonLambda lambda, CurrentFunction currentFunction)
        : base(path, context)
    {
        _lambda = lambda;
        _currentFunction = currentFunction;
    }

    protected override IItem? GetItemImpl()
    {
        try
        {
            var alias = _lambda.GetAlias(_currentFunction.Name, ItemName);
            return new AliasItem(ParentPath, alias, _currentFunction.Name);
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
