using System.Management.Automation;
using Amazon.ECR.Model;
using MountAnything;

namespace MountAws.Services.Ecr;

public class ImageScanFindingItem : AwsItem
{
    public ImageScanFindingItem(ItemPath parentPath, ImageScanFinding finding) : base(parentPath, new PSObject(finding))
    {
        ItemName = finding.Name;
        Severity = finding.Severity.Value;
        FindingType = ScanFindingType.Standard;
    }

    public ImageScanFindingItem(ItemPath parentPath, EnhancedImageScanFinding enhancedFinding) : base(parentPath,
        new PSObject(enhancedFinding))
    {
        ItemName = enhancedFinding.FindingArn.Split("/").Last();
        Severity = enhancedFinding.Severity;
        FindingType = ScanFindingType.Enhanced;
    }

    public override string ItemName { get; }
    public override bool IsContainer => false;
    
    [ItemProperty]
    public string Severity { get; }
    
    [ItemProperty]
    public ScanFindingType FindingType { get; }
}

public enum ScanFindingType
{
    Standard,
    Enhanced
}