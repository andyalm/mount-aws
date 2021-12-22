using System.Management.Automation;

namespace MountAws.Api.Ecr;

public class ListImagesResponse
{
    public PSObject[] ImageIds { get; init; }
    public string NextToken { get; init; }
}