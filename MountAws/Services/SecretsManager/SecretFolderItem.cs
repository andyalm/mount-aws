using System.Management.Automation;
using MountAnything;

namespace MountAws.Services.SecretsManager;

public class SecretFolderItem : AwsItem
{
    public SecretFolderItem(ItemPath parentPath, ItemPath directoryPath) : base(parentPath, new PSObject(new
    {
        Name = directoryPath.Name
    }))
    {
        ItemName = directoryPath.Name;
    }

    public override string ItemName { get; }
    public override bool IsContainer => true;
}
