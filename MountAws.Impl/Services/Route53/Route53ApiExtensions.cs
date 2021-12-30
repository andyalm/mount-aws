using Amazon.Route53;
using Amazon.Route53.Model;
using static MountAws.PagingHelper;

namespace MountAws.Services.Route53;

public static class Route53ApiExtensions
{
    public static IEnumerable<HostedZone> ListHostedZones(this IAmazonRoute53 route53)
    {
        return Paginate(nextToken =>
        {
            var response = route53.ListHostedZonesAsync(new ListHostedZonesRequest
            {
                Marker = nextToken,
                MaxItems = "100"
            }).GetAwaiter().GetResult();

            return (response.HostedZones, response.NextMarker);
        });
    }

    public static HostedZone GetHostedZone(this IAmazonRoute53 route53, string id)
    {
        return route53.GetHostedZoneAsync(new GetHostedZoneRequest
        {
            Id = id
        }).GetAwaiter().GetResult().HostedZone;
    }

    public static IEnumerable<ResourceRecordSet> ListResourceRecordSets(this IAmazonRoute53 route53, string zoneId)
    {
        return Paginate(nextToken =>
        {
            var response = route53.ListResourceRecordSetsAsync(new ListResourceRecordSetsRequest(zoneId)
            {
                MaxItems = "100",
                StartRecordName = nextToken
            }).GetAwaiter().GetResult();

            return (response.ResourceRecordSets, response.NextRecordName);
        });
    }

    public static ResourceRecordSet GetResourceRecordSet(this IAmazonRoute53 route53, string zoneId, string recordName)
    {
        var record = route53.ListResourceRecordSetsAsync(new ListResourceRecordSetsRequest(zoneId)
        {
            MaxItems = "1",
            StartRecordName = recordName
        }).GetAwaiter().GetResult().ResourceRecordSets.SingleOrDefault();

        if (record == null)
        {
            throw new RecordSetNotFoundException(recordName);
        }

        return record;
    }
}