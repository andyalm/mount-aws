using Amazon.ECR;
using Amazon.ECR.Model;
using MountAnything;
using static MountAws.PagingHelper;

namespace MountAws.Services.ECR;

public static class ECRClientExtensions
{
    public static IEnumerable<RepositoryItem> GetChildRepositories(this IAmazonECR ecr, string parentPath, string? prefix = null)
    {
        var allRepositories = GetWithPaging(nextToken =>
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