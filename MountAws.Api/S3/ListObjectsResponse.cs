using System.Management.Automation;

namespace MountAws.Api.S3;

public class ListObjectsResponse
{
    public ListObjectsResponse(string[] commonPrefixes, PSObject[] s3Objects)
    {
        CommonPrefixes = commonPrefixes;
        S3Objects = s3Objects;
    }

    public string[] CommonPrefixes { get; }
    public PSObject[] S3Objects { get; }
}