using System.Management.Automation;
using Amazon.EC2;
using Amazon.EC2.Model;
using MountAnything;
using MountAws.Api.Ec2;
using DescribeInstancesRequest = MountAws.Api.Ec2.DescribeInstancesRequest;
using DescribeSecurityGroupsRequest = MountAws.Api.Ec2.DescribeSecurityGroupsRequest;
using Filter = Amazon.EC2.Model.Filter;

namespace MountAws.Api.AwsSdk.Ec2;

public class AwsSdkEc2Api : IEc2Api
{
    private readonly IAmazonEC2 _ec2;

    public AwsSdkEc2Api(IAmazonEC2 ec2)
    {
        _ec2 = ec2;
    }

    public (IEnumerable<PSObject> Instances, string NextToken) DescribeInstances(DescribeInstancesRequest request)
    {
        var response = _ec2.DescribeInstancesAsync(new Amazon.EC2.Model.DescribeInstancesRequest
        {
            InstanceIds = request.InstanceIds,
            Filters = request.Filters
                .Select(p => new Filter(p.Name, p.Values))
                .ToList(),
            NextToken = request.NextToken,
            MaxResults = 100
        }).GetAwaiter().GetResult();
        
        return (response.Reservations.SelectMany(r => r.Instances).ToPSObjects(),
                response.NextToken);
    }

    public void TerminateInstance(string instanceId)
    {
        _ec2.TerminateInstancesAsync(new TerminateInstancesRequest
        {
            InstanceIds = new List<string> { instanceId }
        }).GetAwaiter().GetResult();
    }

    public (IEnumerable<PSObject> SecurityGroups, string NextToken) DescribeSecurityGroups(DescribeSecurityGroupsRequest request)
    {
        var awsRequest = new Amazon.EC2.Model.DescribeSecurityGroupsRequest
        {
            NextToken = request.NextToken,
        };
        if (request.Filters.Any())
        {
            awsRequest.Filters = request.Filters.Select(f => new Filter(f.Name, f.Values)).ToList();
        }
        if (request.Ids.Any())
        {
            awsRequest.GroupIds.AddRange(request.Ids);
        }
        else
        {
            awsRequest.MaxResults = 100;
        }
        var response = _ec2.DescribeSecurityGroupsAsync(awsRequest).GetAwaiter().GetResult();

        return (response.SecurityGroups.ToPSObjects(),
            response.NextToken);
    }
}