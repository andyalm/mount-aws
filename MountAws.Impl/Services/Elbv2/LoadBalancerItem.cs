using System.Management.Automation;
using Amazon.EC2.Model;
using Amazon.ElasticLoadBalancingV2.Model;
using MountAnything;

namespace MountAws.Services.Elbv2;

public class LoadBalancerItem : AwsItem<LoadBalancer>
{
    public IEnumerable<SecurityGroup> SecurityGroups { get; }
    public string[] SecurityGroupNames { get; }
    public LoadBalancerItem(ItemPath parentPath, LoadBalancer loadBalancer, IEnumerable<SecurityGroup> securityGroups) : base(parentPath, loadBalancer)
    {
        SecurityGroups = securityGroups;
        SecurityGroupNames = securityGroups.Select(g => g.GroupName).ToArray();
    }

    public override string ItemName => UnderlyingObject.LoadBalancerName;
    public override string ItemType => Elbv2ItemTypes.LoadBalancer;
    public override bool IsContainer => true;
    public string LoadBalancerArn => UnderlyingObject.LoadBalancerArn;

    protected override void CustomizePSObject(PSObject psObject)
    {
        var securityGroupIds = UnderlyingObject.SecurityGroups;
        psObject.Properties.Add(new PSNoteProperty("SecurityGroupIds", securityGroupIds));
        psObject.Properties.Remove(nameof(SecurityGroups));
        psObject.Properties.Add(new PSNoteProperty(nameof(SecurityGroups), SecurityGroups));
        psObject.Properties.Add(new PSNoteProperty(nameof(SecurityGroupNames), SecurityGroupNames));
        base.CustomizePSObject(psObject);
    }

    public override string? WebUrl => UrlBuilder.CombineWith($"ec2/v2/home?#LoadBalancers:search={ItemName}");
}