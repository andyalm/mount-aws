using Amazon.Lambda;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Lambda;

public class VersionsHandler : PathHandler
{
    private readonly IAmazonLambda _lambda;
    private readonly CurrentFunction _currentFunction;

    public static Item CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "versions",
            "Navigate the published versions of this Lambda function");
    }

    public VersionsHandler(ItemPath path, IPathHandlerContext context,
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
        return _lambda.ListVersionsByFunction(_currentFunction.Name)
            .Select(v => new VersionItem(Path, v))
            .OrderBy(v => v.ItemName);
    }
}
