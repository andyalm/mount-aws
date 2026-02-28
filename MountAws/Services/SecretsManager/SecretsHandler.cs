using Amazon.SecretsManager;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.SecretsManager;

public class SecretsHandler : PathHandler
{
    private readonly IAmazonSecretsManager _secretsManager;

    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "secrets",
            "Navigate secrets as a virtual filesystem");
    }

    public SecretsHandler(ItemPath path, IPathHandlerContext context, IAmazonSecretsManager secretsManager) : base(path, context)
    {
        _secretsManager = secretsManager;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return _secretsManager.ListSecrets()
            .Select(s => new SecretItem(Path, s));
    }
}
