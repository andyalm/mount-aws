using System.Management.Automation;
using Amazon.ECR;
using Amazon.ECR.Model;
using MountAws.Api.Ecr;
using DescribeRepositoriesResponse = MountAws.Api.Ecr.DescribeRepositoriesResponse;
using ListImagesResponse = MountAws.Api.Ecr.ListImagesResponse;
using RepositoryNotFoundException = MountAws.Api.Ecr.RepositoryNotFoundException;

namespace MountAws.Api.AwsSdk.ECR;

public class AwsSdkEcrApi : IEcrApi
{
    private readonly IAmazonECR _ecr;

    public AwsSdkEcrApi(IAmazonECR ecr)
    {
        _ecr = ecr;
    }

    public PSObject DescribeRepository(string repositoryName)
    {
        try
        {
            return _ecr.DescribeRepositoriesAsync(new DescribeRepositoriesRequest
            {
                RepositoryNames = new List<string>{repositoryName}
            }).GetAwaiter().GetResult().Repositories.Single().ToPSObject();
        }
        catch (Amazon.ECR.Model.RepositoryNotFoundException)
        {
            throw new RepositoryNotFoundException(repositoryName);
        }
    }

    public DescribeRepositoriesResponse DescribeRepositories(string? nextToken = null)
    {
        var response = _ecr.DescribeRepositoriesAsync(new DescribeRepositoriesRequest
        {
            NextToken = nextToken
        }).GetAwaiter().GetResult();

        return new DescribeRepositoriesResponse
        {
            Repositories = response.Repositories.ToPSObjects().ToArray(),
            NextToken = response.NextToken
        };
    }

    public ListImagesResponse ListTaggedImages(string repositoryName, string? nextToken = null)
    {
        var response = _ecr.ListImagesAsync(new ListImagesRequest
        {
            RepositoryName = repositoryName,
            Filter = new ListImagesFilter
            {
                TagStatus = TagStatus.TAGGED
            },
            NextToken = nextToken
        }).GetAwaiter().GetResult();

        return new ListImagesResponse(
            response.ImageIds.ToPSObjects().ToArray(),
            response.NextToken
        );
    }
}