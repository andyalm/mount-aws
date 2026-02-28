using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using MountAnything;

namespace MountAws.Services.SecretsManager;

public class SecretNavigator : ItemNavigator<SecretListEntry, IItem>
{
    private readonly IAmazonSecretsManager _secretsManager;

    public SecretNavigator(IAmazonSecretsManager secretsManager)
    {
        _secretsManager = secretsManager;
    }

    protected override IItem CreateDirectoryItem(ItemPath parentPath, ItemPath directoryPath)
    {
        return new SecretFolderItem(parentPath, directoryPath);
    }

    protected override IItem CreateItem(ItemPath parentPath, SecretListEntry model)
    {
        return new SecretItem(parentPath, model);
    }

    protected override ItemPath GetPath(SecretListEntry model)
    {
        return new ItemPath(model.Name);
    }

    protected override IEnumerable<SecretListEntry> ListItems(ItemPath? pathPrefix)
    {
        return _secretsManager.ListSecrets();
    }
}
