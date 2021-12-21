using Amazon.ECR;
using Amazon.ECR.Model;
using MountAnything;

namespace MountAws.Services.ECR;

public class RepositoryHandler : PathHandler
{
    private readonly IAmazonECR _ecr;
    private readonly RepositoryPath _repositoryPath;

    public RepositoryHandler(string path, IPathHandlerContext context, IAmazonECR ecr, RepositoryPath repositoryPath) : base(path, context)
    {
        _ecr = ecr;
        _repositoryPath = repositoryPath;
    }

    protected override Item? GetItemImpl()
    {
        Repository? repository = null;
        try
        {
            repository = _ecr.DescribeRepositoriesAsync(new DescribeRepositoriesRequest
            {
                RepositoryNames = new List<string>{_repositoryPath.Value}
            }).GetAwaiter().GetResult().Repositories.SingleOrDefault();
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
                    new RepositoryPath(ItemPath.GetParent(_repositoryPath.Value)))
                .GetChildItems().SingleOrDefault(i => i.ItemName.Equals(ItemName, StringComparison.OrdinalIgnoreCase));
        }

        return new ECRRootHandler(ParentPath, Context, _ecr)
            .GetChildItems().SingleOrDefault(i => i.ItemName.Equals(ItemName, StringComparison.OrdinalIgnoreCase));
    }

    protected override IEnumerable<Item> GetChildItemsImpl()
    {
        return _ecr.GetChildRepositories(Path, _repositoryPath.Value);
    }
}