using Amazon.ElasticLoadBalancingV2.Model;
using MountAnything;

namespace MountAws.Services.ELBV2;

public class LoadBalancerItem : Item
{
    public LoadBalancer LoadBalancer { get; }

    public LoadBalancerItem(string parentPath, LoadBalancer loadBalancer) : base(parentPath)
    {
        LoadBalancer = loadBalancer;
    }

    public override string ItemName => LoadBalancer.LoadBalancerName;
    public override object UnderlyingObject => LoadBalancer;
    public override string ItemType => "LoadBalancer";
    public override bool IsContainer => true;
}