using System.Management.Automation;
using MountAnything;

namespace MountAws.Services.SecretsManager;

public class SecretFolderItem(ItemPath parentPath, ItemPath directoryPath)
    : AwsItem(parentPath.Combine(directoryPath).Parent, new PSObject())
{
    public override string ItemName { get; } = directoryPath.Name;
    public override bool IsContainer => true;
    protected override string TypeName => typeof(SecretItem).FullName!;
    public override string ItemType => SecretsManagerItemTypes.Directory;
}
