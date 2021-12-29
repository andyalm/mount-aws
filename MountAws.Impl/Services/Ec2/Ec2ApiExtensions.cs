using System.Net;
using System.Text.RegularExpressions;
using Amazon.EC2;
using Amazon.EC2.Model;
using MountAws.Api.Ec2;

using static MountAws.PagingHelper;

namespace MountAws.Services.Ec2;

public static class Ec2ApiExtensions
{
    public static IEnumerable<Instance> QueryInstances(this IAmazonEC2 client, string? filter = null)
    {
        var request = !string.IsNullOrEmpty(filter) ? ParseInstanceFilter(filter) : new DescribeInstancesRequest();
        
        return GetWithPaging(nextToken =>
        {
            request.NextToken = nextToken;
            
            var response = client.DescribeInstances(request);

            return new PaginatedResponse<Instance>
            {
                PageOfResults = response.Instances.ToArray(),
                NextToken = response.NextToken
            };
        });
    }

    public static IEnumerable<Instance> GetInstancesByIds(this IAmazonEC2 ec2, IEnumerable<string> instanceIds)
    {
        var instanceIdList = instanceIds.ToList();
        if (instanceIdList.Count == 0)
        {
            return Enumerable.Empty<Instance>();
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
                request.Filters.Add(new Filter(parts[0], new List<string>{parts[1]}));
            }
            else
            {
                request.Filters.Add(new Filter("tag:Name", new List<string>{filter}));
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
                request.Filters.Add(new Filter("group-id", new List<string>{filter}));
            }
            else if (filter.StartsWith("sg-"))
            {
                request.GroupIds.Add(filter);
            }
            else if (filter.Contains('='))
            {
                var parts = filter.Split("=");
                request.Filters.Add(new Filter(parts[0], new List<string>{parts[1]}));
            }
            else
            {
                request.Filters.Add(new Filter("group-name", new List<string>{filter}));
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
    
    public static (IEnumerable<Instance> Instances, string NextToken) DescribeInstances(this IAmazonEC2 ec2, DescribeInstancesRequest request)
    {
        // only set max results if no filters applied (it sometimes doesn't return results)
        if(!request.Filters.Any() && !request.InstanceIds.Any())
        {
            request.MaxResults = 100;
        }
        var response = ec2.DescribeInstancesAsync(request).GetAwaiter().GetResult();
        
        return (response.Reservations.SelectMany(r => r.Instances),
                response.NextToken);
    }

    public static void TerminateInstance(this IAmazonEC2 ec2, string instanceId)
    {
        ec2.TerminateInstancesAsync(new TerminateInstancesRequest
        {
            InstanceIds = new List<string> { instanceId }
        }).GetAwaiter().GetResult();
    }

    public static IEnumerable<Vpc> DescribeVpcs(this IAmazonEC2 ec2)
    {
        return ec2.DescribeVpcsAsync(new DescribeVpcsRequest())
            .GetAwaiter().GetResult().Vpcs;
    }

    public static Vpc DescribeVpc(this IAmazonEC2 ec2, string vpcId)
    {
        try
        {
            return ec2.DescribeVpcsAsync(new DescribeVpcsRequest { VpcIds = new List<string> { vpcId } })
                .GetAwaiter().GetResult().Vpcs.Single();
        }
        catch (AmazonEC2Exception ex) when (ex.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.BadRequest)
        {
            throw new VpcNotFoundException(vpcId);
        }
    }

    public static (IEnumerable<SecurityGroup> SecurityGroups, string NextToken) DescribeSecurityGroups(this IAmazonEC2 ec2, DescribeSecurityGroupsRequest request)
    {
        // only set max results if no filters applied (it sometimes doesn't return results)
        if(!request.Filters.Any() && !request.GroupIds.Any())
        {
            request.MaxResults = 100;
        }

        if (!string.IsNullOrEmpty(request.NextToken))
        {
            request.NextToken = request.NextToken;
        }
        var response = ec2.DescribeSecurityGroupsAsync(request).GetAwaiter().GetResult();
        return (response.SecurityGroups, response.NextToken);
    }

    public static IEnumerable<Subnet> DescribeSubnetsByVpc(this IAmazonEC2 ec2, string vpcId)
    {
        return ec2.DescribeSubnetsAsync(new DescribeSubnetsRequest
        {
            Filters = new List<Filter> { new Filter("vpc-id", new List<string> { vpcId }) }
        }).GetAwaiter().GetResult().Subnets;
    }
    
    public static Subnet DescribeSubnet(this IAmazonEC2 ec2, string subnetId)
    {
        try
        {
            return ec2.DescribeSubnetsAsync(new DescribeSubnetsRequest
            {
                SubnetIds = new List<string>{subnetId}
            }).GetAwaiter().GetResult().Subnets.Single();
        }
        catch (AmazonEC2Exception ex) when(ex.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.BadRequest)
        {
            throw new SubnetNotFoundException(subnetId);
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
            new List<string>{ResolveEc2IPAddress(ipAddress)});
    }
}