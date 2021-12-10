using System.Collections;
using System.Text.RegularExpressions;
using Amazon.EC2;
using Amazon.EC2.Model;

namespace MountAws.Services.EC2;

public static class EC2ClientExtensions
{
    public static IEnumerable<Instance> QueryInstances(this IAmazonEC2 client, string filter, IEC2QueryParameters? parameters = null)
    {
        var request = new DescribeInstancesRequest();
        if (!string.IsNullOrEmpty(filter))
        {
            request.Filters.AddRange(ParseFilters(filter));
        }

        if (parameters?.IPAddress != null)
        {
            request.Filters.Add(IPAddressFilter(parameters.IPAddress));
        }
        var response = client.DescribeInstancesAsync(request).GetAwaiter().GetResult();

        return response.Reservations.SelectMany(r => r.Instances);
    }

    private static Filter IPAddressFilter(string ipAddress)
    {
        return new Filter("network-interface.addresses.private-ip-address",
            new List<string> { ResolveEc2IPAddress(ipAddress) });
    }

    public static string? Name(this Instance instance)
    {
        return instance.Tags
            .SingleOrDefault(t => t.Key.Equals("Name", StringComparison.OrdinalIgnoreCase))?.Value;
    }

    private static readonly Regex _ipHostName =
        new Regex(@"^ip-(?<first>\d+)-(?<second>\d+)-(?<third>\d+)-(?<fourth>\d+)");

    private static readonly Regex _ipv4 = new Regex(@"\d+\.\d+\.\d+\.\d+");
    private static string ResolveEc2IPAddress(string ip)
    {
        var hostNameMatch = _ipHostName.Match(ip);
        if (hostNameMatch.Success)
        {
            return
                $"{hostNameMatch.Groups["first"].Value}.{hostNameMatch.Groups["second"].Value}.{hostNameMatch.Groups["third"].Value}.{hostNameMatch.Groups["fourth"].Value}";
        }

        return ip;
    }

    public static Filter ParseFilter(string filter)
    {
        if (filter.StartsWith("i-"))
        {
            return new Filter(filter);
        }

        if (_ipv4.IsMatch(filter) || _ipHostName.IsMatch(filter))
        {
            return IPAddressFilter(filter);
        }
        
        if (filter.Contains('='))
        {
            var parts = filter.Split("=");
            return new Filter(parts[0], new List<string> { parts[1] });
        }

        return new Filter("tag:Name", new List<string> { filter });
    }

    private static IEnumerable<Filter> ParseFilters(string filterString)
    {
        return filterString.Split(",").Select(ParseFilter);
    }
}