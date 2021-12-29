using System.Management.Automation;
using MountAnything;
using MountAws.Api.Ecr;

namespace MountAws.Services.Ecr;

public class RepositoryHandler : PathHandler
{
    private readonly IEcrApi _ecr;
    private readonly RepositoryPath _repositoryPath;

    public RepositoryHandler(string path, IPathHandlerContext context, IEcrApi ecr, RepositoryPath repositoryPath) : base(path, context)
    {
        _ecr = ecr;
        _repositoryPath = repositoryPath;
    }

    protected override IItem? GetItemImpl()
    {
        PSObject? repository = null;
        try
        {
            repository = _ecr.DescribeRepository(_repositoryPath.Value);
        }
        catch (Api.Ecr.RepositoryNotFoundException ex)
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
                    new RepositoryPath(ItemPath.GetParent(_repositoryPath.Value)))
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