using System.Management.Automation;

namespace MountAws.Services.ELBV2;

public class TargetGroupItem : AwsItem
{

    public TargetGroupItem(string parentPath, PSObject targetGroup) : base(parentPath, targetGroup) {}

    public override string ItemName => Property<string>("TargetGroupName")!;
    public override string ItemType => Elbv2ItemTypes.TargetGroup;
    public override bool IsContainer => true;

    public string TargetGroupArn => Property<string>("TargetGroupArn")!;
}

public class WeightedTargetGroupItem : TargetGroupItem
{
    public int Weight { get; }
    
    public WeightedTargetGroupItem(string parentPath, PSObject targetGroup, int weight) : base(parentPath, targetGroup)
    {
        Weight = weight;
    }

    public override void CustomizePSObject(PSObject psObject)
    {
        psObject.Properties.Add(new PSNoteProperty("Weight", Weight));
    }
}