using System.Management.Automation;
using Amazon.Route53;
using Amazon.Route53.Model;
using MountAnything;
using MountAws.Api.Route53;
using HostedZoneNotFoundException = MountAws.Api.Route53.HostedZoneNotFoundException;

namespace MountAws.Api.AwsSdk.Route53;

public class AwsSdkRoute53Api : IRoute53Api
{
    private readonly IAmazonRoute53 _route53;

    public AwsSdkRoute53Api(IAmazonRoute53 route53)
    {
        _route53 = route53;
    }

    public (IEnumerable<PSObject> HostedZones, string? NextToken) ListHostedZones(string? nextToken)
    {
        var response = _route53.ListHostedZonesAsync(new ListHostedZonesRequest
        {
            Marker = nextToken,
            MaxItems = "100"
        }).GetAwaiter().GetResult();

        return (response.HostedZones.ToPSObjects(), response.Marker);
    }

    public PSObject GetHostedZone(string id)
    {
        try
        {
            return _route53.GetHostedZoneAsync(new GetHostedZoneRequest
            {
                Id = id
            }).GetAwaiter().GetResult().HostedZone.ToPSObject();
        }
        catch (Amazon.Route53.Model.HostedZoneNotFoundException)
        {
            throw new HostedZoneNotFoundException(id);
        }
    }

    public (IEnumerable<PSObject> Records, string? NextToken) ListResourceRecordSets(string zoneId, string? nextToken)
    {
        var response = _route53.ListResourceRecordSetsAsync(new ListResourceRecordSetsRequest(zoneId)
        {
            MaxItems = "100",
            StartRecordName = nextToken
        }).GetAwaiter().GetResult();

        return (response.ResourceRecordSets.ToPSObjects(), response.NextRecordName);
    }

    public PSObject GetResourceRecordSet(string zoneId, string recordName)
    {
        var record = _route53.ListResourceRecordSetsAsync(new ListResourceRecordSetsRequest(zoneId)
        {
            MaxItems = "1",
            StartRecordName = recordName
        }).GetAwaiter().GetResult().ResourceRecordSets.SingleOrDefault()?.ToPSObject();

        if (record == null)
        {
            throw new RecordSetNotFoundException(recordName);
        }

        return record;
    }
}