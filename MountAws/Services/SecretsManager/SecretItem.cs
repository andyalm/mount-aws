using System.Management.Automation;
using Amazon.SecretsManager.Model;
using MountAnything;

namespace MountAws.Services.SecretsManager;

public class SecretItem : AwsItem
{
    public SecretItem(ItemPath parentPath, SecretListEntry secret) : base(parentPath, new PSObject(secret))
    {
        ItemName = new ItemPath(secret.Name).Name;
        SecretName = secret.Name;
        ItemType = SecretsManagerItemTypes.Secret;
        Description = secret.Description;
        Arn = secret.ARN;
        LastChangedDate = secret.LastChangedDate;
        CreatedDate = secret.CreatedDate;
    }

    public SecretItem(ItemPath parentPath, DescribeSecretResponse secret) : base(parentPath, new PSObject(secret))
    {
        ItemName = new ItemPath(secret.Name).Name;
        SecretName = secret.Name;
        ItemType = SecretsManagerItemTypes.Secret;
        Description = secret.Description;
        Arn = secret.ARN;
        LastChangedDate = secret.LastChangedDate;
        CreatedDate = secret.CreatedDate;
    }

    public override string ItemName { get; }
    public override bool IsContainer => false;
    protected override string TypeName => GetType().FullName!;
    public override string ItemType { get; }
    [ItemProperty]
    public string Name => ItemName;
    [ItemProperty]
    public string SecretName { get; }
    [ItemProperty]
    public string? Description { get; }
    [ItemProperty]
    public string Arn { get; }
    [ItemProperty]
    public DateTime? LastChangedDate { get; }
    [ItemProperty]
    public DateTime? CreatedDate { get; }

    public override string? WebUrl =>
        UrlBuilder.CombineWith($"secretsmanager/secret?name={Uri.EscapeDataString(SecretName)}");
}
