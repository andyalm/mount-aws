using Amazon.ECR;
using Amazon.ECR.Model;
using MountAnything;

namespace MountAws.Services.Ecr;

public class ImageScanItem : AwsItem<DescribeImageScanFindingsResponse>
{
    public ImageScanItem(ItemPath parentPath, DescribeImageScanFindingsResponse underlyingObject) : base(parentPath, underlyingObject)
    {
        ItemName = "image-scan";
    }

    public override string ItemName { get; }
    
    public override bool IsContainer => true;

    [ItemProperty] 
    public string StatusDescription => UnderlyingObject.ImageScanStatus.Description;

    [ItemProperty]
    public string Status => UnderlyingObject.ImageScanStatus.Status.Value;

    [ItemProperty]
    public int CriticalCount => UnderlyingObject.ImageScanFindings.FindingSeverityCounts.GetValueOrDefault(FindingSeverity.CRITICAL, 0);
    [ItemProperty]
    public int HighCount => UnderlyingObject.ImageScanFindings.FindingSeverityCounts.GetValueOrDefault(FindingSeverity.HIGH, 0);
    [ItemProperty]
    public int MediumCount => UnderlyingObject.ImageScanFindings.FindingSeverityCounts.GetValueOrDefault(FindingSeverity.MEDIUM, 0);
    [ItemProperty]
    public int LowCount => UnderlyingObject.ImageScanFindings.FindingSeverityCounts.GetValueOrDefault(FindingSeverity.LOW, 0);
    [ItemProperty]
    public int InformationalCount => UnderlyingObject.ImageScanFindings.FindingSeverityCounts.GetValueOrDefault(FindingSeverity.INFORMATIONAL, 0);
}