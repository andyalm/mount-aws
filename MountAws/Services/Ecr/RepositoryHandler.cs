using Amazon.ECR;
using Amazon.ECR.Model;
using MountAnything;

namespace MountAws.Services.Ecr;

public class RepositoryHandler : PathHandler
{
    private readonly IAmazonECR _ecr;
    private readonly RepositoryPath _repositoryPath;

    public RepositoryHandler(ItemPath path, IPathHandlerContext context, IAmazonECR ecr, RepositoryPath repositoryPath) : base(path, context)
    {
        _ecr = ecr;
        _repositoryPath = repositoryPath;
    }

    protected override IItem? GetItemImpl()
    {
        Repository? repository = null;
        try
        {
            repository = _ecr.DescribeRepository(_repositoryPath.Value);
        }
        catch (RepositoryNotFoundException ex)
        {
            WriteDebug(ex.ToString());
        }

        if (repository != null)
        {
            return new RepositoryItem(ParentPath, repository);
        }

        if (_repositoryPath.Value.Contains(ItemPath.Separator))
        {
            return new RepositoryHandler(ParentPath, Context, _ecr,
                    new RepositoryPath(new ItemPath(_repositoryPath.Value).Parent.FullName))
                .GetChildItems().SingleOrDefault(i => i.ItemName.Equals(ItemName, StringComparison.OrdinalIgnoreCase));
        }

        return new EcrRootHandler(ParentPath, Context, _ecr)
            .GetChildItems().SingleOrDefault(i => i.ItemName.Equals(ItemName, StringComparison.OrdinalIgnoreCase));
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        var item = GetItem() as RepositoryItem;
        if (item?.ItemType == EcrItemTypes.Repository)
        {
            return new[] { ImageTagsHandler.CreateItem(Path) };
        }
        
        return _ecr.GetChildRepositories(Path, _repositoryPath.Value);
    }
}