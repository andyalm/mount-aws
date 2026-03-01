using System.Management.Automation;
using Amazon.SecretsManager.Model;
using MountAnything;

namespace MountAws.Services.SecretsManager;

public class SecretItem(ItemPath parentPath, SecretListEntry secret) : AwsItem(parentPath, new PSObject(secret))
{
    public override string ItemName { get; } = new ItemPath(secret.Name).Name;
    public override bool IsContainer => false;
    protected override string TypeName => GetType().FullName!;
    public override string ItemType => SecretsManagerItemTypes.Secret;

    [ItemProperty]
    public string Name => ItemName;
    [ItemProperty]
    public string SecretName => secret.Name;
    [ItemProperty]
    public string Arn => secret.ARN;

    public override string? WebUrl =>
        UrlBuilder.CombineWith($"secretsmanager/secret?name={Uri.EscapeDataString(SecretName)}");
}
