using System.Management.Automation;

namespace MountAws.Api.S3;

public class ListObjectsResponse
{
    public string[] CommonPrefixes { get; init; }
    public PSObject[] S3Objects { get; init; }
}