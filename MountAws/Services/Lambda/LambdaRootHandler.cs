using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Lambda;

public class LambdaRootHandler : PathHandler
{
    public static Item CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "lambda",
            "Navigate Lambda functions, layers, and related resources");
    }

    public LambdaRootHandler(ItemPath path, IPathHandlerContext context) : base(path, context)
    {
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield return FunctionsHandler.CreateItem(Path);
        yield return LayersHandler.CreateItem(Path);
    }
}
