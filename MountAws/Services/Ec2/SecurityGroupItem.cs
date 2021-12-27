using System.Management.Automation;
using MountAnything;

namespace MountAws.Services.Ec2;

public class SecurityGroupItem : AwsItem
{
    public SecurityGroupItem(string parentPath, PSObject securityGroup) : base(parentPath, securityGroup)
    {
        ItemName = securityGroup.Property<string>("GroupId")!;
    }

    public override string ItemName { get; }
    public override bool IsContainer => true;
    public string GroupName => Property<string>(nameof(GroupName))!;
    public override void CustomizePSObject(PSObject psObject)
    {
        psObject.Properties.Add(new PSAliasProperty("Id", "GroupId"));
        psObject.Properties.Add(new PSAliasProperty("Name", nameof(GroupName)));
        base.CustomizePSObject(psObject);
    }
}