using Amazon.Lambda;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Lambda;

public class AliasesHandler : PathHandler
{
    private readonly IAmazonLambda _lambda;
    private readonly CurrentFunction _currentFunction;

    public static Item CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "aliases",
            "Navigate the aliases for this Lambda function");
    }

    public AliasesHandler(ItemPath path, IPathHandlerContext context,
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
        return _lambda.ListAliases(_currentFunction.Name)
            .Select(a => new AliasItem(Path, a, _currentFunction.Name))
            .OrderBy(a => a.ItemName);
    }
}
