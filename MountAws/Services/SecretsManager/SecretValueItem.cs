using System.Management.Automation;
using MountAnything;

namespace MountAws.Services.SecretsManager;

public class SecretValueItem(ItemPath parentPath, string secretName, string key) : AwsItem(parentPath, new PSObject())
{
    public override string ItemName => key;
    [ItemProperty]
    public string SecretName { get; } = secretName;
    public override bool IsContainer => false;
    public override string ItemType => SecretsManagerItemTypes.SecretValue;

    [ItemProperty] public string Key => key;
}
