using Amazon.ECR;
using Amazon.ECR.Model;
using MountAnything;

namespace MountAws.Services.Ecr;

public class ImageScanHandler : PathHandler
{
    private readonly IAmazonECR _ecr;
    private readonly RepositoryPath _repositoryPath;

    public ImageScanHandler(ItemPath path, IPathHandlerContext context, IAmazonECR ecr, RepositoryPath repositoryPath) : base(path, context)
    {
        _ecr = ecr;
        _repositoryPath = repositoryPath;
    }

    protected override IItem? GetItemImpl()
    {
        try
        {
            var findings = _ecr.DescribeImageScanFindings(_repositoryPath.Value, ParentPath.Name);

            return new ImageScanItem(Path, findings);
        }
        catch (RepositoryNotFoundException)
        {
            return null;
        }
        
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        if (GetItem() is not ImageScanItem scan)
        {
            return Enumerable.Empty<IItem>();
        }
        
        var standardFindings =
            scan.UnderlyingObject.ImageScanFindings?.Findings.Select(f => new ImageScanFindingItem(Path, f)) ?? Enumerable.Empty<ImageScanFindingItem>();

        var enhancedFindings =
            scan.UnderlyingObject.ImageScanFindings?.EnhancedFindings.Select(f =>
                new ImageScanFindingItem(Path, f)) ?? Enumerable.Empty<ImageScanFindingItem>();

        return standardFindings.Concat(enhancedFindings).OrderBy(f => f.Severity);
    }
}