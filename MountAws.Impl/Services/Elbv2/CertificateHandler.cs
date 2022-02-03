using Amazon;
using Amazon.CertificateManager;
using MountAnything;
using MountAws.Services.Acm;
using MountAws.Services.Core;

namespace MountAws.Services.Elbv2;

public class CertificateHandler : PathHandler
{
    private readonly IAmazonCertificateManager _acm;
    private readonly CurrentRegion _region;
    private readonly CallerIdentity _callerIdentity;
    private readonly IItemAncestor<ListenerItem> _listenerItem;

    public CertificateHandler(ItemPath path, IPathHandlerContext context, IAmazonCertificateManager acm,
        CurrentRegion region, CallerIdentity callerIdentity, IItemAncestor<ListenerItem> listenerItem) : base(path, context)
    {
        _acm = acm;
        _region = region;
        _callerIdentity = callerIdentity;
        _listenerItem = listenerItem;
    }

    protected override IItem? GetItemImpl()
    {
        var arn = $"arn:aws:acm:{_region.Value}:{_callerIdentity.AccountId}:certificate/{ItemName}";

        var certificate = _acm.DescribeCertificate(arn);
        var isDefault =
            _listenerItem.Item.UnderlyingObject.Certificates.FirstOrDefault(c => c.IsDefault)?.CertificateArn ==
            certificate.CertificateArn;

        return new CertificateItem(ParentPath, certificate, isDefault);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield break;
    }
}