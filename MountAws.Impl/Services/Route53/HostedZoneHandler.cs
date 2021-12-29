using System.Management.Automation;
using Amazon.Route53;
using Amazon.Route53.Model;
using MountAnything;
using static MountAws.PagingHelper;

namespace MountAws.Services.Route53;

public class HostedZoneHandler : PathHandler
{
    private readonly IAmazonRoute53 _route53;

    public HostedZoneHandler(string path, IPathHandlerContext context, IAmazonRoute53 route53) : base(path, context)
    {
        _route53 = route53;
    }

    protected override IItem? GetItemImpl()
    {
        try
        {
            var hostedZone = _route53.GetHostedZone(ItemName);
            return new HostedZoneItem(ParentPath, hostedZone);
        }
        catch (HostedZoneNotFoundException)
        {
            return null;
        }
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return _route53.ListResourceRecordSets(ItemName)
            .Select(r => new ResourceRecordItem(Path, r));
    }
}