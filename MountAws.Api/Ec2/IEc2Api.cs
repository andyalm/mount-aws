using System.Management.Automation;

namespace MountAws.Api.Ec2;

public interface IEc2Api
{
    (IEnumerable<PSObject> Instances, string NextToken) DescribeInstances(DescribeInstancesRequest request);
    void TerminateInstance(string instanceId);
    (IEnumerable<PSObject> SecurityGroups, string NextToken) DescribeSecurityGroups(DescribeSecurityGroupsRequest request);
    IEnumerable<PSObject> DescribeVpcs();
    PSObject DescribeVpc(string vpcId);
    IEnumerable<PSObject> DescribeSubnetsByVpc(string vpcId);
    PSObject DescribeSubnet(string subnetId);
}