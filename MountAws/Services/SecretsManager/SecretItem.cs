using Amazon.SecretsManager.Model;
using MountAnything;

namespace MountAws.Services.SecretsManager;

public class SecretItem : AwsItem<SecretListEntry>
{
    public SecretItem(ItemPath parentPath, SecretListEntry secret) : base(parentPath, secret)
    {
        ItemName = secret.Name;
    }

    public override string ItemName { get; }
    public override bool IsContainer => true;

    public string Name => UnderlyingObject.Name;
    public string? Description => UnderlyingObject.Description;
    public string ARN => UnderlyingObject.ARN;
    public DateTime? LastChangedDate => UnderlyingObject.LastChangedDate;
    public DateTime? CreatedDate => UnderlyingObject.CreatedDate;

    public override string? WebUrl =>
        UrlBuilder.CombineWith($"secretsmanager/secret?name={Uri.EscapeDataString(ItemName)}");
}
