namespace MountAws.Api.S3;

public class ListObjectsRequest
{
    public string? BucketName { get; init; }
    public string? Prefix { get; set; }
    public string? Delimiter { get; set; }
    public int? MaxKeys { get; set; }
}