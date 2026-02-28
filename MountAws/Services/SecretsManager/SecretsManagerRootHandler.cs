using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.SecretsManager;

public class SecretsManagerRootHandler : PathHandler
{
    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "secretsmanager",
            "Navigate AWS Secrets Manager secrets in a virtual filesystem");
    }

    public SecretsManagerRootHandler(ItemPath path, IPathHandlerContext context) : base(path, context)
    {
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield return SecretsHandler.CreateItem(Path);
    }
}
