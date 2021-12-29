using System.Management.Automation;

namespace MountAws.Services.Ec2;

public class SubnetItem : AwsItem
{
    public SubnetItem(string parentPath, PSObject subnet) : base(parentPath, subnet)
    {
        ItemName = Property<string>("SubnetId")!;
    }

    public override string ItemName { get; }
    public override bool IsContainer => false;
}