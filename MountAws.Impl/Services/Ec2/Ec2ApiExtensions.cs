using System.Management.Automation;
using System.Text.RegularExpressions;
using MountAws.Api.Ec2;
using DescribeInstancesRequest = MountAws.Api.Ec2.DescribeInstancesRequest;

using static MountAws.PagingHelper;

namespace MountAws.Services.Ec2;

public static class Ec2ApiExtensions
{
    public static IEnumerable<PSObject> QueryInstances(this IEc2Api client, string? filter = null)
    {
        var request = !string.IsNullOrEmpty(filter) ? ParseInstanceFilter(filter) : new DescribeInstancesRequest();
        
        return GetWithPaging(nextToken =>
        {
            request.NextToken = nextToken;
            
            var response = client.DescribeInstances(request);

            return new PaginatedResponse<PSObject>
            {
                PageOfResults = response.Instances.ToArray(),
                NextToken = response.NextToken
            };
        });
    }

    public static IEnumerable<PSObject> GetInstancesByIds(this IEc2Api ec2, IEnumerable<string> instanceIds)
    {
        var instanceIdList = instanceIds.ToList();
        if (instanceIdList.Count == 0)
        {
            return Enumerable.Empty<PSObject>();
        }
        
        return ec2.DescribeInstances(new DescribeInstancesRequest
            {
                InstanceIds = instanceIdList
            }).Instances;
    }

    public static DescribeInstancesRequest ParseInstanceFilter(string filterString)
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
                request.Filters.Add(new Filter(parts[0], parts[1]));
            }
            else
            {
                request.Filters.Add(new Filter("tag:Name", filter));
            }
        }

        return request;
    }
    
    public static DescribeSecurityGroupsRequest ParseSecurityGroupFilter(string filterString)
    {
        var request = new DescribeSecurityGroupsRequest();
        foreach (var filter in filterString.Split(","))
        {
            if (filter.StartsWith("sg-") && filter.Contains("*"))
            {
                request.Filters.Add(new Filter("group-id", filter));
            }
            else if (filter.StartsWith("sg-"))
            {
                request.Ids.Add(filter);
            }
            else if (filter.Contains('='))
            {
                var parts = filter.Split("=");
                request.Filters.Add(new Filter(parts[0], parts[1]));
            }
            else
            {
                request.Filters.Add(new Filter("group-name", filter));
            }
        }

        return request;
    }

    private static void AddInstanceIdFilter(DescribeInstancesRequest request, string filter)
    {
        if (filter.Contains("*"))
        {
            request.Filters.Add(new Filter("instance-id", filter));
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
            ResolveEc2IPAddress(ipAddress));
    }
}