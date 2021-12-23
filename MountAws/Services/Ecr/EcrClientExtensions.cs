using System.Management.Automation;
using MountAnything;
using MountAws.Api;
using MountAws.Api.Ecr;
using static MountAws.PagingHelper;

namespace MountAws.Services.Ecr;

public static class EcrClientExtensions
{
    public static IEnumerable<RepositoryItem> GetChildRepositories(this IEcrApi ecr, string parentPath, string? prefix = null)
    {
        var allRepositories = GetWithPaging(nextToken =>
        {
            var response = ecr.DescribeRepositories(nextToken);

            return new PaginatedResponse<PSObject>
            {
                PageOfResults = response.Repositories,
                NextToken = response.NextToken
            };
        });
        if (prefix == null)
        {
            return allRepositories.Select(r =>
            {
                if (r.Property<string>("RepositoryName")!.Contains(ItemPath.Separator))
                {
                    return new RepositoryItem(parentPath,
                        r.Property<string>("RepositoryName")!.Split(ItemPath.Separator.ToString()).First());
                }
                else
                {
                    return new RepositoryItem(parentPath, r);
                }
            }).DistinctBy(r => r.ItemName);
        }

        return allRepositories.Where(r => r.Property<string>("RepositoryName")!.StartsWith($"{prefix}{ItemPath.Separator}"))
            .Select(r =>
            {
                var itemName = r.Property<string>("RepositoryName")!.Substring(prefix.Length + 1);
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