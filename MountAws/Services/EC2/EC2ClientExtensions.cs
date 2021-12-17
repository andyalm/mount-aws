using System.Collections;
using System.Text.RegularExpressions;
using Amazon.EC2;
using Amazon.EC2.Model;

namespace MountAws.Services.EC2;

public static class EC2ClientExtensions
{
    public static IEnumerable<Instance> QueryInstances(this IAmazonEC2 client, string? filter = null)
    {
        var request = !string.IsNullOrEmpty(filter) ? ParseFilter(filter) : new DescribeInstancesRequest();
        
        var response = client.DescribeInstancesAsync(request).GetAwaiter().GetResult();

        return response.Reservations.SelectMany(r => r.Instances);
    }

    public static string? Name(this Instance instance)
    {
        return instance.Tags
            .SingleOrDefault(t => t.Key.Equals("Name", StringComparison.OrdinalIgnoreCase))?.Value;
    }

    public static IEnumerable<Instance> GetInstancesByIds(this IAmazonEC2 ec2, IEnumerable<string> instanceIds)
    {
        var instanceIdList = instanceIds.ToList();
        if (instanceIdList.Count == 0)
        {
            return Enumerable.Empty<Instance>();
        }
        
        var response = ec2.DescribeInstancesAsync(new DescribeInstancesRequest
            {
                InstanceIds = instanceIdList
            }).GetAwaiter().GetResult();

        return response.Reservations.SelectMany(r => r.Instances);
    }

    public static DescribeInstancesRequest ParseFilter(string filterString)
    {
        var request = new DescribeInstancesRequest();
        foreach (var filter in filterString.Split(","))
        {
            if (filter.StartsWith("i-"))
            {
                AddInstanceIdFilter(request, filter);
            }
            else if (_ipv4.IsMatch(filter) || _ipHostName.IsMatch(filter))
            {
                request.Filters.Add(IPAddressFilter(filter));
            }
            else if (filter.Contains('='))
            {
                var parts = filter.Split("=");
                request.Filters.Add(new Filter(parts[0], new List<string> { parts[1] }));
            }
            else
            {
                request.Filters.Add(new Filter("tag:Name", new List<string> { filter }));
            }
        }

        return request;
    }

    private static void AddInstanceIdFilter(DescribeInstancesRequest request, string filter)
    {
        if (filter.Contains("*"))
        {
            request.Filters.Add(new Filter("instance-id", new List<string>{filter}));
        }
        else
        {
            request.InstanceIds.Add(filter);
        }
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
    
    private static Filter IPAddressFilter(string ipAddress)
    {
        return new Filter("network-interface.addresses.private-ip-address",
            new List<string> { ResolveEc2IPAddress(ipAddress) });
    }
}