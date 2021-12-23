using System.Management.Automation;

namespace MountAws.Services.ELBV2;

public class LoadBalancerItem : AwsItem
{
    public LoadBalancerItem(string parentPath, PSObject loadBalancer) : base(parentPath, loadBalancer) {}

    public override string ItemName => Property<string>("LoadBalancerName")!;
    public override string ItemType => Elbv2ItemTypes.LoadBalancer;
    public override bool IsContainer => true;
    public string LoadBalancerArn => Property<string>(nameof(LoadBalancerArn))!;
}