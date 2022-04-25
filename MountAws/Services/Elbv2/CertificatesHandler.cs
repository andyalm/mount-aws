using Amazon.CertificateManager;
using Amazon.ElasticLoadBalancingV2;
using Amazon.ServiceDiscovery;
using MountAnything;
using MountAws.Services.Acm;
using MountAws.Services.Core;

namespace MountAws.Services.Elbv2;

public class CertificatesHandler : PathHandler
{
    private readonly IAmazonCertificateManager _acm;
    private readonly IAmazonElasticLoadBalancingV2 _elbv2;
    private readonly IItemAncestor<ListenerItem> _listenerItem;

    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "certificates",
            "Lists the certificates attached to the load balancer");
    }
    
    public CertificatesHandler(ItemPath path, IPathHandlerContext context,
        IAmazonCertificateManager acm,
        IAmazonElasticLoadBalancingV2 elbv2,
        IItemAncestor<ListenerItem> listenerItem) : base(path, context)
    {
        _acm = acm;
        _elbv2 = elbv2;
        _listenerItem = listenerItem;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return _elbv2.DescribeListenerCertificates(_listenerItem.Item.ListenerArn)
            .Select(c => new CertificateItem(Path, _acm.DescribeCertificate(c.CertificateArn), c.IsDefault));
    }
}