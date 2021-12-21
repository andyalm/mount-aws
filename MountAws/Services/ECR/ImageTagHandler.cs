using Amazon.ECR;
using MountAnything;

namespace MountAws.Services.ECR;

public class ImageTagHandler : PathHandler
{
    private readonly IAmazonECR _ecr;
    private readonly RepositoryPath _repositoryPath;

    public ImageTagHandler(string path, IPathHandlerContext context, IAmazonECR ecr, RepositoryPath repositoryPath) : base(path, context)
    {
        _ecr = ecr;
        _repositoryPath = repositoryPath;
    }

    protected override Item? GetItemImpl()
    {
        var parentHandler = new ImageTagsHandler(ParentPath, Context, _ecr, _repositoryPath);
        return parentHandler.GetChildItems()
            .FirstOrDefault(i => i.ItemName.Equals(ItemName, StringComparison.OrdinalIgnoreCase));
    }

    protected override IEnumerable<Item> GetChildItemsImpl()
    {
        yield break;
    }
}