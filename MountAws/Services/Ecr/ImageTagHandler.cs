using MountAnything;
using MountAws.Api.Ecr;

namespace MountAws.Services.Ecr;

public class ImageTagHandler : PathHandler
{
    private readonly IEcrApi _ecr;
    private readonly RepositoryPath _repositoryPath;

    public ImageTagHandler(string path, IPathHandlerContext context, IEcrApi ecr, RepositoryPath repositoryPath) : base(path, context)
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
        yield break;
    }
}