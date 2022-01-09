using System.Management.Automation;
using Amazon.ElasticLoadBalancingV2.Model;
using MountAnything;

namespace MountAws.Services.Elbv2;

public class TargetGroupItem : AwsItem<TargetGroup>
{

    public TargetGroupItem(ItemPath parentPath, TargetGroup targetGroup) : base(parentPath, targetGroup) {}

    public override string ItemName => UnderlyingObject.TargetGroupName;
    public override string ItemType => Elbv2ItemTypes.TargetGroup;
    public override bool IsContainer => true;
    public string TargetGroupArn => UnderlyingObject.TargetGroupArn;
    public override string? WebUrl =>
        UrlBuilder.CombineWith($"ec2/home#TargetGroup:targetGroupArn={TargetGroupArn}");
}

public class WeightedTargetGroupItem : TargetGroupItem
{
    public int Weight { get; }
    
    public WeightedTargetGroupItem(ItemPath parentPath, TargetGroup targetGroup, int weight) : base(parentPath, targetGroup)
    {
        Weight = weight;
    }

    protected override void CustomizePSObject(PSObject psObject)
    {
        psObject.Properties.Add(new PSNoteProperty("Weight", Weight));
    }
}