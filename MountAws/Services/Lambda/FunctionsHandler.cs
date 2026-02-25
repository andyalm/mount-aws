using Amazon.Lambda;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Lambda;

public class FunctionsHandler : PathHandler
{
    private readonly IAmazonLambda _lambda;

    public static Item CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "functions",
            "Navigate Lambda functions in this account and region");
    }

    public FunctionsHandler(ItemPath path, IPathHandlerContext context, IAmazonLambda lambda)
        : base(path, context)
    {
        _lambda = lambda;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return _lambda.ListFunctions()
            .Select(f => new FunctionItem(Path, f))
            .OrderBy(f => f.ItemName);
    }
}
