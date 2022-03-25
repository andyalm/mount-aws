using Amazon.ECR;
using MountAnything;

namespace MountAws.Services.Ecr;

public class ImageScanFindingHandler : PathHandler
{
    private readonly ImageScanHandler _parentHandler;
    
    public ImageScanFindingHandler(ItemPath path, IPathHandlerContext context, IAmazonECR ecr, RepositoryPath repositoryPath) : base(path, context)
    {
        _parentHandler = new ImageScanHandler(path.Parent, context, ecr, repositoryPath);
    }

    protected override IItem? GetItemImpl()
    {
        return _parentHandler.GetChildItems()
            .SingleOrDefault(i => i.ItemName.Equals(ItemName, StringComparison.OrdinalIgnoreCase));
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield break;
    }
}