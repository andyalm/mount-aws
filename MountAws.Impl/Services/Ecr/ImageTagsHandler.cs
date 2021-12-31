using Amazon.ECR;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Ecr;

public class ImageTagsHandler : PathHandler
{
    public static Item CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "image-tags",
            "Navigate the docker image tags for this repository");
    }
    
    private readonly IAmazonECR _ecr;
    private readonly RepositoryPath _repositoryPath;

    public ImageTagsHandler(ItemPath path, IPathHandlerContext context, IAmazonECR ecr, RepositoryPath repositoryPath) : base(path, context)
    {
        _ecr = ecr;
        _repositoryPath = repositoryPath;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        var repositoryHandler = new RepositoryHandler(ParentPath, Context, _ecr, _repositoryPath);
        var repositoryItem = repositoryHandler.GetItem(Freshness.Default) as RepositoryItem;
        if (repositoryItem?.ItemType != EcrItemTypes.Repository)
        {
            return Enumerable.Empty<Item>();
        }
        
        return _ecr.ListTaggedImages(_repositoryPath.Value)
            .Select(i => new ImageTagItem(Path, i, repositoryItem.Repository!));
    }
}