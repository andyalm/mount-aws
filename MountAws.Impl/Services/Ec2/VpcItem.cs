using System.Management.Automation;
using Amazon.EC2.Model;

namespace MountAws.Services.Ec2;

public class VpcItem : AwsItem<Vpc>
{
    public VpcItem(string parentPath, Vpc underlyingObject) : base(parentPath, underlyingObject)
    {
        ItemName = UnderlyingObject.VpcId;
    }

    public override string ItemName { get; }
    public override bool IsContainer => true;
}