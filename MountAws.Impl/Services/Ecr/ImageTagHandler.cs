using Amazon.ECR;
using MountAnything;

namespace MountAws.Services.Ecr;

public class ImageTagHandler : PathHandler
{
    private readonly IAmazonECR _ecr;
    private readonly RepositoryPath _repositoryPath;

    public ImageTagHandler(ItemPath path, IPathHandlerContext context, IAmazonECR ecr, RepositoryPath repositoryPath) : base(path, context)
    {
        _ecr = ecr;
        _repositoryPath = repositoryPath;
    }

    protected override IItem? GetItemImpl()
    {
        var parentHandler = new ImageTagsHandler(ParentPath, Context, _ecr, _repositoryPath);
        return parentHandler.GetChildItems(Freshness.Default)
            .FirstOrDefault(i => i.ItemName.Equals(ItemName, StringComparison.OrdinalIgnoreCase));
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        var findings = _ecr.DescribeImageScanFindings(_repositoryPath.Value, ItemName);

        yield return new ImageScanItem(Path, findings);
    }
}