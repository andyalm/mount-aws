using System.Management.Automation;
using MountAnything;

namespace MountAws.Services.Elbv2;

public class LoadBalancerItem : AwsItem
{
    public LoadBalancerItem(string parentPath, PSObject loadBalancer) : base(parentPath, loadBalancer) {}

    public override string ItemName => Property<string>("LoadBalancerName")!;
    public override string ItemType => Elbv2ItemTypes.LoadBalancer;
    public override bool IsContainer => true;
    public string LoadBalancerArn => Property<string>(nameof(LoadBalancerArn))!;
}