using Amazon.CertificateManager.Model;
using MountAnything;

namespace MountAws.Services.Elbv2;

public class CertificateItem : AwsItem<CertificateDetail>
{
    public CertificateItem(ItemPath parentPath, CertificateDetail certificate, bool isDefault) : base(parentPath, certificate)
    {
        IsDefault = isDefault;
        ItemName = certificate.CertificateArn.Split("/")[^1];
    }

    public override string ItemName { get; }

    [ItemProperty]
    public string Id => ItemName;

    [ItemProperty]
    public bool IsDefault { get; }

    public override bool IsContainer => false;
}