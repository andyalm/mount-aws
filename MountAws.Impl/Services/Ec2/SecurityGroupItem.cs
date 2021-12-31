using System.Management.Automation;
using Amazon.EC2.Model;
using MountAnything;

namespace MountAws.Services.Ec2;

public class SecurityGroupItem : AwsItem<SecurityGroup>
{
    public SecurityGroupItem(ItemPath parentPath, SecurityGroup securityGroup) : base(parentPath, securityGroup)
    {
        ItemName = securityGroup.GroupId;
    }

    public override string ItemName { get; }
    public override bool IsContainer => true;
    public string GroupName => UnderlyingObject.GroupName;
    public override void CustomizePSObject(PSObject psObject)
    {
        psObject.Properties.Add(new PSAliasProperty("Id", nameof(SecurityGroup.GroupId)));
        psObject.Properties.Add(new PSAliasProperty("Name", nameof(GroupName)));
        base.CustomizePSObject(psObject);
    }

    public override IEnumerable<string> Aliases
    {
        get
        {
            yield return GroupName;
        }
    }
}