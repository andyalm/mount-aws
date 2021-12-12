using Amazon.ElasticLoadBalancingV2.Model;

namespace MountAws.Services.ELBV2;

public class LoadBalancerItem : AwsItem
{
    private readonly string _parentPath;
    public LoadBalancer LoadBalancer { get; }

    public LoadBalancerItem(string parentPath, LoadBalancer loadBalancer)
    {
        _parentPath = parentPath;
        LoadBalancer = loadBalancer;
    }

    public override string FullPath => AwsPath.Combine(_parentPath, ItemName);
    public override string ItemName => LoadBalancer.LoadBalancerName;
    public override object UnderlyingObject => LoadBalancer;
    public override string ItemType => "LoadBalancer";
    public override bool IsContainer => true;
}