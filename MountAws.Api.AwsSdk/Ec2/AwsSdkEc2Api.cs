using System.Management.Automation;
using Amazon.EC2;
using Amazon.EC2.Model;
using MountAnything;
using MountAws.Api.Ec2;
using DescribeInstancesRequest = MountAws.Api.Ec2.DescribeInstancesRequest;

namespace MountAws.Api.AwsSdk.Ec2;

public class AwsSdkEc2Api : IEc2Api
{
    private readonly IAmazonEC2 _ec2;

    public AwsSdkEc2Api(IAmazonEC2 ec2)
    {
        _ec2 = ec2;
    }

    public IEnumerable<PSObject> DescribeInstances(DescribeInstancesRequest request)
    {
        return _ec2.DescribeInstancesAsync(new Amazon.EC2.Model.DescribeInstancesRequest
        {
            InstanceIds = request.InstanceIds,
            Filters = request.Filters
                .Select(p => new Filter(p.Key, new List<string> { p.Value }))
                .ToList()
        }).GetAwaiter().GetResult().Reservations.SelectMany(r => r.Instances).ToPSObjects();
    }
}