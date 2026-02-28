using System.Management.Automation;
using Amazon.SecretsManager.Model;
using MountAnything;

namespace MountAws.Services.SecretsManager;

public class SecretItem : AwsItem
{
    public SecretItem(ItemPath parentPath, SecretListEntry secret) : base(parentPath, new PSObject(secret))
    {
        ItemName = new ItemPath(secret.Name).Name;
        Name = secret.Name;
        Description = secret.Description;
        ARN = secret.ARN;
        LastChangedDate = secret.LastChangedDate;
        CreatedDate = secret.CreatedDate;
    }

    public SecretItem(ItemPath parentPath, DescribeSecretResponse secret) : base(parentPath, new PSObject(secret))
    {
        ItemName = new ItemPath(secret.Name).Name;
        Name = secret.Name;
        Description = secret.Description;
        ARN = secret.ARN;
        LastChangedDate = secret.LastChangedDate;
        CreatedDate = secret.CreatedDate;
    }

    public override string ItemName { get; }
    public override bool IsContainer => true;

    public string Name { get; }
    public string? Description { get; }
    public string ARN { get; }
    public DateTime? LastChangedDate { get; }
    public DateTime? CreatedDate { get; }

    public override string? WebUrl =>
        UrlBuilder.CombineWith($"secretsmanager/secret?name={Uri.EscapeDataString(Name)}");
}
