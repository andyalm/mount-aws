using System.Management.Automation;
using Amazon.ElasticLoadBalancingV2.Model;

namespace MountAws.Services.ELBV2;

public class TargetGroupItem : AwsItem
{
    private readonly string _parentPath;
    public TargetGroup TargetGroup { get; }

    public TargetGroupItem(string parentPath, TargetGroup targetGroup)
    {
        _parentPath = parentPath;
        TargetGroup = targetGroup;
    }

    public override string FullPath => AwsPath.Combine(_parentPath, ItemName);
    public override string ItemName => TargetGroup.TargetGroupName;
    public override object UnderlyingObject => TargetGroup;
    public override string ItemType => "TargetGroup";
    public override bool IsContainer => true;
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