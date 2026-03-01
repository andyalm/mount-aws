using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.SecretsManager;

public class SecretsHandler : PathHandler
{
    private readonly SecretNavigator _navigator;

    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "secrets",
            "Navigate secrets as a virtual filesystem");
    }

    public SecretsHandler(ItemPath path, IPathHandlerContext context, SecretNavigator navigator) : base(path, context)
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
