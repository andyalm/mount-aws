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
        return Paginate(nextToken =>
        {
            var response = ecr.DescribeRepositoriesAsync(new DescribeRepositoriesRequest
            {
                NextToken = nextToken
            }).GetAwaiter().GetResult();

            return (response.Repositories, response.NextToken);
        });
    }

    public static IEnumerable<ImageIdentifier> ListTaggedImages(this IAmazonECR ecr, string repositoryName)
    {
        return Paginate(nextToken =>
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

            return (response.ImageIds, response.NextToken);
        });
    }
    
    public static IEnumerable<RepositoryItem> GetChildRepositories(this IAmazonECR ecr, ItemPath parentPath, string? prefix = null)
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

    public static DescribeImageScanFindingsResponse DescribeImageScanFindings(this IAmazonECR ecr, string repositoryName, string imageTag)
    {
        DescribeImageScanFindingsResponse? response = null;
        while(true)
        {
            var pageResponse = ecr.DescribeImageScanFindingsAsync(new DescribeImageScanFindingsRequest
            {
                RepositoryName = repositoryName,
                ImageId = new ImageIdentifier
                {
                    ImageTag = imageTag
                },
                NextToken = response?.NextToken
            }).GetAwaiter().GetResult();
            if (response == null)
            {
                response = pageResponse;
            }
            else
            {
                response.ImageScanStatus = pageResponse.ImageScanStatus;
                response.ImageScanFindings.Findings.AddRange(pageResponse.ImageScanFindings.Findings);
                response.ImageScanFindings.EnhancedFindings.AddRange(pageResponse.ImageScanFindings.EnhancedFindings);
                response.ImageScanFindings.ImageScanCompletedAt = pageResponse.ImageScanFindings.ImageScanCompletedAt;
                response.ImageScanFindings.VulnerabilitySourceUpdatedAt = pageResponse.ImageScanFindings.VulnerabilitySourceUpdatedAt;
                response.ImageScanFindings.MergeSeverityCounts(pageResponse.ImageScanFindings);
            }

            response.NextToken = pageResponse.NextToken;
            if (response.NextToken == null)
            {
                break;
            }
        }

        return response;
    }
}