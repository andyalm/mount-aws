using System.Management.Automation;
using Amazon.ElasticLoadBalancingV2.Model;
using MountAnything;

namespace MountAws.Services.Elbv2;

public class TargetGroupItem : AwsItem<TargetGroup>
{

    public TargetGroupItem(string parentPath, TargetGroup targetGroup) : base(parentPath, targetGroup) {}

    public override string ItemName => UnderlyingObject.TargetGroupName;
    public override string ItemType => Elbv2ItemTypes.TargetGroup;
    public override bool IsContainer => true;
    public string TargetGroupArn => UnderlyingObject.TargetGroupArn;
}

public class WeightedTargetGroupItem : TargetGroupItem
{
    public int Weight { get; }
    
    public WeightedTargetGroupItem(string parentPath, TargetGroup targetGroup, int weight) : base(parentPath, targetGroup)
    {
        Weight = weight;
    }

    public override void CustomizePSObject(PSObject psObject)
    {
        psObject.Properties.Add(new PSNoteProperty("Weight", Weight));
    }
}