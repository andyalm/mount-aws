using System.Management.Automation;

namespace MountAws.Api.Ec2;

public interface IEc2Api
{
    IEnumerable<PSObject> DescribeInstances(DescribeInstancesRequest request);
    void TerminateInstance(string instanceId); 
}