using Amazon.ECR;
using Amazon.ECR.Model;
using MountAnything;
using static MountAws.PagingHelper;

namespace MountAws.Services.Ecr;

public static class EcrApiExtensions
{
    public static Repository DescribeRepository(this IAmazonECR ecr, string repositoryName)
    {
        return ecr.DescribeRepositoriesAsync(new DescribeRepositoriesRequest
        {
            RepositoryNames = new List<string> { repositoryName }
        }).GetAwaiter().GetResult().Repositories.Single();
    }

    public static IEnumerable<Repository> DescribeRepositories(this IAmazonECR ecr)
    {
        return GetWithPaging(nextToken =>
        {
            var response = ecr.DescribeRepositoriesAsync(new DescribeRepositoriesRequest
            {
                NextToken = nextToken
            }).GetAwaiter().GetResult();

            return new PaginatedResponse<Repository>
            {
                PageOfResults = response.Repositories.ToArray(),
                NextToken = response.NextToken
            };
        });
    }

    public static IEnumerable<ImageIdentifier> ListTaggedImages(this IAmazonECR ecr, string repositoryName, string? nextToken = null)
    {
        return GetWithPaging(nextToken =>
        {
            var response = ecr.ListImagesAsync(new ListImagesRequest
            {
                RepositoryName = repositoryName,
                Filter = new ListImagesFilter
                {
                    TagStatus = TagStatus.TAGGED
                },
                NextToken = nextToken
            }).GetAwaiter().GetResult();

            return new PaginatedResponse<ImageIdentifier>
            {
                PageOfResults = response.ImageIds.ToArray(),
                NextToken = response.NextToken
            };
        });
    }
    
    public static IEnumerable<RepositoryItem> GetChildRepositories(this IAmazonECR ecr, string parentPath, string? prefix = null)
    {
        var allRepositories = ecr.DescribeRepositories();
        if (prefix == null)
        {
            return allRepositories.Select(r =>
            {
                if (r.RepositoryName.Contains(ItemPath.Separator))
                {
                    return new RepositoryItem(parentPath,
                        r.RepositoryName.Split(ItemPath.Separator.ToString()).First());
                }
                else
                {
                    return new RepositoryItem(parentPath, r);
                }
            }).DistinctBy(r => r.ItemName);
        }

        return allRepositories.Where(r => r.RepositoryName.StartsWith($"{prefix}{ItemPath.Separator}"))
            .Select(r =>
            {
                var itemName = r.RepositoryName.Substring(prefix.Length + 1);
                if (itemName.Contains(ItemPath.Separator))
                {
                    return new RepositoryItem(parentPath, itemName.Split(ItemPath.Separator.ToString()).First());
                }
                else
                {
                    return new RepositoryItem(parentPath, r);
                }
            }).DistinctBy(r => r.ItemName);
    }
}