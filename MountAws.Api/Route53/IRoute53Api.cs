using System.Management.Automation;

namespace MountAws.Api.Route53;

public interface IRoute53Api
{
    (IEnumerable<PSObject> HostedZones, string? NextToken) ListHostedZones(string? nextToken);
    PSObject GetHostedZone(string id);
    (IEnumerable<PSObject> Records, string? NextToken) ListResourceRecordSets(string zoneId, string? nextToken);
    PSObject GetResourceRecordSet(string zoneId, string recordName);
}