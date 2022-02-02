using Amazon.Route53;
using Amazon.Route53.Model;
using MountAnything;

namespace MountAws.Services.Route53;

public class HostedZoneHandler : PathHandler
{
    private readonly IAmazonRoute53 _route53;

    public HostedZoneHandler(ItemPath path, IPathHandlerContext context, IAmazonRoute53 route53) : base(path, context)
    {
        _route53 = route53;
    }

    private string ZoneId => Cache.ResolveAlias<HostedZoneItem>(ItemName, HostedZoneIdFromName);

    protected override IItem? GetItemImpl()
    {
        try
        {
            var hostedZone = _route53.GetHostedZone(ZoneId);
            return new HostedZoneItem(ParentPath, hostedZone);
        }
        catch (HostedZoneNotFoundException)
        {
            return null;
        }
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return _route53.ListResourceRecordSets(ZoneId)
            .Select(r => new ResourceRecordItem(Path, r));
    }
    
    private string HostedZoneIdFromName(string hostedZoneName)
    {
        var hostedZone = _route53.ListHostedZones()
            .SingleOrDefault(z => z.Name.Equals(hostedZoneName, StringComparison.OrdinalIgnoreCase));
        if (hostedZone == null)
        {
            throw new HostedZoneNotFoundException($"A hosted zone with name '{hostedZoneName}' does not exist");
        }

        return hostedZone.Name;
    }
}