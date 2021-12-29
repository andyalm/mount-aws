using System.Management.Automation;

namespace MountAws.Services.Ec2;

public class VpcItem : AwsItem
{
    public VpcItem(string parentPath, PSObject underlyingObject) : base(parentPath, underlyingObject)
    {
        ItemName = Property<string>("VpcId")!;
    }

    public override string ItemName { get; }
    public override bool IsContainer => true;
}