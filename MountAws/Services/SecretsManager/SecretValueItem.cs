using System.Management.Automation;
using MountAnything;

namespace MountAws.Services.SecretsManager;

public class SecretValueItem : AwsItem
{
    public SecretValueItem(ItemPath parentPath, string key, string value) : base(parentPath, new PSObject(new
    {
        Key = key,
        Value = value
    }))
    {
        ItemName = key;
        Value = value;
    }

    public override string ItemName { get; }
    public override bool IsContainer => false;
    public string Value { get; }
}
