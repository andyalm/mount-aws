using System.Management.Automation;
using MountAnything;

namespace MountAws.Services.Elbv2;

public class LoadBalancerItem : AwsItem
{
    public IEnumerable<PSObject> SecurityGroups { get; }
    public string[] SecurityGroupNames { get; }
    public LoadBalancerItem(string parentPath, PSObject loadBalancer, IEnumerable<PSObject> securityGroups) : base(parentPath, loadBalancer)
    {
        SecurityGroups = securityGroups;
        SecurityGroupNames = securityGroups.Select(g => g.Property<string>("GroupName")!).ToArray();
    }

    public override string ItemName => Property<string>("LoadBalancerName")!;
    public override string ItemType => Elbv2ItemTypes.LoadBalancer;
    public override bool IsContainer => true;
    public string LoadBalancerArn => Property<string>(nameof(LoadBalancerArn))!;

    public override void CustomizePSObject(PSObject psObject)
    {
        var securityGroupIds = psObject.Property<IEnumerable<string>>("SecurityGroups")!;
        psObject.Properties.Add(new PSNoteProperty("SecurityGroupIds", securityGroupIds));
        psObject.Properties.Remove(nameof(SecurityGroups));
        psObject.Properties.Add(new PSNoteProperty(nameof(SecurityGroups), SecurityGroups));
        psObject.Properties.Add(new PSNoteProperty(nameof(SecurityGroupNames), SecurityGroupNames));
        base.CustomizePSObject(psObject);
    }
}