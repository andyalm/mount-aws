using System.Management.Automation;
using Amazon.EC2.Model;
using MountAnything;

namespace MountAws.Services.Ec2;

public class SubnetItem : AwsItem<Subnet>
{
    public SubnetItem(ItemPath parentPath, Subnet subnet) : base(parentPath, subnet)
    {
        ItemName = UnderlyingObject.SubnetId;
    }

    public override string ItemName { get; }
    public override bool IsContainer => false;
}