using System.Management.Automation;

namespace MountAws.Api.Ecr;

public class ListImagesResponse
{
    public ListImagesResponse(PSObject[] imageIds, string nextToken)
    {
        ImageIds = imageIds;
        NextToken = nextToken;
    }

    public PSObject[] ImageIds { get;  }
    public string NextToken { get; }
}