using System.Management.Automation;

namespace MountAws.Api.Ecr;

public interface IEcrApi
{
    PSObject? DescribeRepository(string repositoryName);
    
    DescribeRepositoriesResponse DescribeRepositories(string? nextToken = null);

    ListImagesResponse ListTaggedImages(string repositoryName, string? nextToken = null);
}